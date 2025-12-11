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
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(0.5); // kaj - hoevaak de loop gebeurt terwijl de backend draait.

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

            var auctions = await db.Auctions
                .AsNoTracking()
                .Select(a => new { a.auction_id, a.status, a.start_time, a.end_time })
                .ToListAsync(ct);

            _logger.LogInformation("Loaded {Count} auctions.", auctions?.Count ?? 0);

            var changes = 0;

            foreach (var a in auctions)
            {
                DateTime startUtc = a.start_time.Kind switch
                {
                    DateTimeKind.Utc => a.start_time,
                    DateTimeKind.Local => a.start_time.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(a.start_time, DateTimeKind.Utc)
                };

                DateTime endUtc = a.end_time == default
                    ? DateTime.MaxValue
                    : a.end_time.Kind switch
                    {
                        DateTimeKind.Utc => a.end_time,
                        DateTimeKind.Local => a.end_time.ToUniversalTime(),
                        _ => DateTime.SpecifyKind(a.end_time, DateTimeKind.Utc)
                    };

                string expected =
                    nowUtc < startUtc ? "upcoming" :
                    nowUtc < endUtc ? "active" :
                    "completed";

                _logger.LogInformation("Auction {Id}: DB='{DBStatus}', expected='{Expected}', startUtc={Start:O}, endUtc={End}",
                    a.auction_id,
                    a.status ?? "<null>",
                    expected,
                    startUtc,
                    endUtc == DateTime.MaxValue ? (object)"MaxValue" : endUtc);

                if (!string.Equals(a.status ?? string.Empty, expected, StringComparison.OrdinalIgnoreCase))
                {
                    var stub = new Auction { auction_id = a.auction_id, status = expected };
                    db.Attach(stub);
                    db.Entry(stub).Property(x => x.status).IsModified = true;

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
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency conflict while saving auction statuses.");
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


        public async Task RunStatusUpdateForTestingAsync(DBContext db, DateTime now)
        {
            var auctions = await db.Auctions.ToListAsync();
            foreach (var auction in auctions)
            {
                if (auction.start_time > now)
                    auction.status = "upcoming";
                else if (auction.start_time <= now && auction.end_time > now)
                    auction.status = "active";
                else
                    auction.status = "closed";
            }
            await db.SaveChangesAsync();
        }
    }
}