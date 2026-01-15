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

    public class AuctionStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<AuctionStatusUpdater> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public AuctionStatusUpdater(IServiceProvider provider, ILogger<AuctionStatusUpdater> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuctionStatusUpdater started");

            using var timer = new PeriodicTimer(_interval);
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        await UpdateStatusesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in auction status update cycle");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("AuctionStatusUpdater stopping");
            }
        }

        private async Task UpdateStatusesAsync(CancellationToken ct)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            var nowUtc = DateTime.UtcNow;

            var auctionsToCheck = await db.Auctions
                .Where(a => a.status != "completed")
                .Select(a => new { a.auction_id, a.status, a.start_time, a.duration_minutes })
                .ToListAsync(ct);

            if (auctionsToCheck.Count == 0)
            {
                return;
            }

            var statusUpdates = new List<(int auctionId, string newStatus)>();

            foreach (var auction in auctionsToCheck)
            {
                // Convert local time (UTC+1) to UTC for comparison
                var startTimeUtc = DateTime.SpecifyKind(auction.start_time, DateTimeKind.Local).ToUniversalTime();
                var endTime = startTimeUtc.AddMinutes(auction.duration_minutes);
                var newStatus = DetermineStatus(nowUtc, startTimeUtc, endTime);

                if (newStatus != auction.status)
                {
                    statusUpdates.Add((auction.auction_id, newStatus));
                    _logger.LogInformation(
                        "Auction {AuctionId}: {OldStatus} → {NewStatus}",
                        auction.auction_id,
                        auction.status,
                        newStatus);
                }
            }

            // Apply all status changes at once
            if (statusUpdates.Count > 0)
            {
                await ApplyStatusUpdatesAsync(db, statusUpdates, ct);
            }
        }

        private static string DetermineStatus(DateTime now, DateTime startTime, DateTime endTime)
        {
            if (now < startTime)
                return "upcoming";
            if (now < endTime)
                return "active";
            return "completed";
        }

        private async Task ApplyStatusUpdatesAsync(
            DBContext db,
            List<(int auctionId, string newStatus)> updates,
            CancellationToken ct)
        {
            try
            {
                foreach (var (auctionId, newStatus) in updates)
                {
                    await db.Auctions
                        .Where(a => a.auction_id == auctionId)
                        .ExecuteUpdateAsync(
                            setters => setters.SetProperty(a => a.status, newStatus),
                            ct);
                }

                _logger.LogInformation("Updated {Count} auction status changes", updates.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update auction statuses");
                throw;
            }
        }


        public async Task RunStatusUpdateForTestingAsync(DBContext db, DateTime testNow)
        {
            var auctions = await db.Auctions.ToListAsync();
            
            foreach (var auction in auctions)
            {
                var endTime = auction.start_time.AddMinutes(auction.duration_minutes);
                auction.status = DetermineStatus(testNow, auction.start_time, endTime);
            }

            await db.SaveChangesAsync();
        }
    }
}