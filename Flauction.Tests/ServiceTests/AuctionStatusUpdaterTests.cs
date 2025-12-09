using System;
using System.Threading.Tasks;
using Flauction.Data;
using Flauction.Models;
using Flauction.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flauction.Tests.ServiceTests
{
    public class AuctionStatusUpdaterTests
    {
        private const string StatusActive = "active";
        private const string StatusUpcoming = "upcoming";
        private const string StatusEnded = "ended";
        private const string StatusSold = "sold";
        private const string StatusNotSold = "not sold";

        private (DBContext, IServiceProvider) CreateInMemoryDbAndServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddDbContext<DBContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            // Add a mock logger to the service collection
            services.AddSingleton<ILogger<AuctionStatusUpdater>>(Mock.Of<ILogger<AuctionStatusUpdater>>());

            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<DBContext>();

            return (dbContext, serviceProvider);
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_AuctionStartsInTheFuture_UpdatesStatusToUpcoming()
        {
            // Arrange
            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
            var now = DateTime.UtcNow;

            db.Auctions.Add(new Auction
            {
                auction_id = 1,
                auctionmaster_id = "test-master-id",
                plant_id = 1,
                status = StatusActive,
                start_time = now.AddMinutes(10),
                end_time = now.AddHours(1),
            });

            await db.SaveChangesAsync();

            // Use the real service provider to create the updater
            var updater = new AuctionStatusUpdater(
                serviceProvider,
                serviceProvider.GetRequiredService<ILogger<AuctionStatusUpdater>>()
            );

            // Act
            await updater.RunStatusUpdateForTestingAsync(db, now);

            // Assert
            var auction = await db.Auctions.FirstAsync();
            Assert.Equal(StatusUpcoming, auction.status);
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_AuctionIsCurrentlyRunning_UpdatesStatusToActive()
        {
            // Arrange
            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
            var now = DateTime.UtcNow;

            db.Auctions.Add(new Auction
            {
                auction_id = 1,
                status = StatusUpcoming,
                start_time = now.AddMinutes(-10),
                end_time = now.AddHours(1),
            });
            await db.SaveChangesAsync();

            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

            // Act
            await updater.RunStatusUpdateForTestingAsync(db, now);

            // Assert
            var auction = await db.Auctions.FirstAsync();
            Assert.Equal(StatusActive, auction.status);
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_AuctionHasEnded_UpdatesStatusToEnded()
        {
            // Arrange
            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
            var now = DateTime.UtcNow;

            db.Auctions.Add(new Auction
            {
                auction_id = 1,
                status = StatusActive,
                start_time = now.AddHours(-2),
                end_time = now.AddHours(-1),
            });
            await db.SaveChangesAsync();

            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

            // Act
            await updater.RunStatusUpdateForTestingAsync(db, now);

            // Assert
            var auction = await db.Auctions.FirstAsync();
            Assert.Equal(StatusEnded, auction.status);
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_EndedEnglishAuctionWithBids_UpdatesStatusToSold()
        {
            // Arrange
            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
            var now = DateTime.UtcNow;

            var auction = new Auction
            {
                auction_id = 1,
                status = StatusEnded,
                start_time = now.AddHours(-2),
                end_time = now.AddHours(-1),
            };
            db.Auctions.Add(auction);
            db.Acceptances.Add(new Acceptance { auction_id = 1, accepted_price = 100 });
            await db.SaveChangesAsync();

            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

            // Act
            await updater.RunStatusUpdateForTestingAsync(db, now);

            // Assert
            var resultAuction = await db.Auctions.FirstAsync();
            Assert.Equal(StatusSold, resultAuction.status);
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_EndedEnglishAuctionWithoutBids_UpdatesStatusToNotSold()
        {
            // Arrange
            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
            var now = DateTime.UtcNow;

            db.Auctions.Add(new Auction
            {
                auction_id = 1,
                status = StatusEnded,
                start_time = now.AddHours(-2),
                end_time = now.AddHours(-1),
            });
            await db.SaveChangesAsync();

            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

            // Act
            await updater.RunStatusUpdateForTestingAsync(db, now);

            // Assert
            var auction = await db.Auctions.FirstAsync();
            Assert.Equal(StatusNotSold, auction.status);
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_EndedDutchAuction_UpdatesStatusToEnded()
        {
            // Arrange
            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
            var now = DateTime.UtcNow;

            db.Auctions.Add(new Auction
            {
                auction_id = 1,
                status = StatusActive,
                start_time = now.AddHours(-2),
                end_time = now.AddHours(-1),
            });
            await db.SaveChangesAsync();

            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

            // Act
            await updater.RunStatusUpdateForTestingAsync(db, now);

            // Assert
            var auction = await db.Auctions.FirstAsync();
            Assert.Equal(StatusEnded, auction.status);
        }
    }
}
