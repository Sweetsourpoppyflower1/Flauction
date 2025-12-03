using System;
using System.Threading;
using System.Threading.Tasks;
using Flauction.Data;
using Flauction.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flauction.Services
{
    // kaj - een updater om de status van Auctions automatisch aantepassen, hierdoor wordt er dus gecheckt of het upcoming is, active of completed.
    public class AuctionStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<AuctionStatusUpdater> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(60); // kaj - hoevaak de loop gebeurt terwijl de backend draait.

        public AuctionStatusUpdater(IServiceProvider provider, ILogger<AuctionStatusUpdater> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuctionStatusUpdater starting.");
            await UpdateStatusesAsync(stoppingToken);

            using var timer = new PeriodicTimer(_interval);
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await UpdateStatusesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) { }

            _logger.LogInformation("AuctionStatusUpdater stopping.");
        }

        private async Task UpdateStatusesAsync(CancellationToken ct)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            var nowUtc = DateTime.UtcNow;
            _logger.LogInformation("UpdateStatusesAsync started (UTC now: {Now:O})", nowUtc);

            var auctions = await db.Set<Auction>().ToListAsync(ct);
            _logger.LogInformation("Loaded {Count} auctions.", auctions?.Count ?? 0);

            var changes = 0;

            foreach (var a in auctions)
            {
                // kaj - checkt de tijden
                var startUtc = a.au_start_time.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(a.au_start_time, DateTimeKind.Local).ToUniversalTime()
                    : a.au_start_time.ToUniversalTime();

                var endUtc = a.au_end_time == default
                    ? DateTime.MaxValue
                    : (a.au_end_time.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(a.au_end_time, DateTimeKind.Local).ToUniversalTime()
                        : a.au_end_time.ToUniversalTime());

                string expected =
                    nowUtc < startUtc ? "upcoming" :
                    nowUtc < endUtc ? "active" :
                    "completed";

                // kaj - simpele code om in de logs te kijken wat er wordt veranderd
                _logger.LogInformation("Auction {Id}: DB='{DBStatus}', expected='{Expected}', startUtc={Start:O}, endUtc={End:O}",
                    a.auction_id,
                    a.au_status ?? "<null>",
                    expected,
                    startUtc,
                    endUtc == DateTime.MaxValue ? (object)"MaxValue" : endUtc);

                if (!string.Equals(a.au_status ?? string.Empty, expected, StringComparison.OrdinalIgnoreCase))
                {
                    a.au_status = expected;
                    db.Entry(a).Property(x => x.au_status).IsModified = true;
                    changes++;
                    _logger.LogInformation("Marked auction {Id} to change to '{Expected}'", a.auction_id, expected);
                }
            }

            if (changes > 0)
            {
                try
                {
                    await db.SaveChangesAsync(ct);
                    _logger.LogInformation("Saved {Count} auction status changes.", changes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed saving auction statuses.");
                }
            }
            else
            {
                _logger.LogInformation("No auction status changes required.");
            }
        }
    }
}