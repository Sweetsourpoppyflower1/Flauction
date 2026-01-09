//using System;
//using System.Threading.Tasks;
//using Flauction.Data;
//using Flauction.Models;
//using Flauction.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Xunit;

//namespace Flauction.Tests.ServiceTests
//{
//    public class AuctionStatusUpdaterTests
//    {
//        private const string StatusActive = "active";
//        private const string StatusUpcoming = "upcoming";
//        private const string StatusEnded = "ended";
//        private const string StatusSold = "sold";
//        private const string StatusNotSold = "not sold";

//        private (DBContext, IServiceProvider) CreateInMemoryDbAndServiceProvider()
//        {
//            var services = new ServiceCollection();
//            services.AddDbContext<DBContext>(options =>
//                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

//            // Add a mock logger to the service collection
//            services.AddSingleton<ILogger<AuctionStatusUpdater>>(Mock.Of<ILogger<AuctionStatusUpdater>>());

//            var serviceProvider = services.BuildServiceProvider();
//            var dbContext = serviceProvider.GetRequiredService<DBContext>();

//            return (dbContext, serviceProvider);
//        }

//        [Fact]
//        public async Task RunStatusUpdateForTestingAsync_AuctionStartsInTheFuture_UpdatesStatusToUpcoming()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                auctionmaster_id = "test-master-id",
//                plant_id = 1,
//                status = StatusActive,
//                start_time = now.AddMinutes(10),
//                end_time = now.AddHours(1),
//            });

//            await db.SaveChangesAsync();

//            // Use the real service provider to create the updater
//            var updater = new AuctionStatusUpdater(
//                serviceProvider,
//                serviceProvider.GetRequiredService<ILogger<AuctionStatusUpdater>>()
//            );

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal(StatusUpcoming, auction.status);
//        }

//        [Fact]
//        public async Task RunStatusUpdateForTestingAsync_AuctionIsCurrentlyRunning_UpdatesStatusToActive()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = StatusUpcoming,
//                start_time = now.AddMinutes(-10),
//                end_time = now.AddHours(1),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal(StatusActive, auction.status);
//        }

//        [Fact]
//        public async Task RunStatusUpdateForTestingAsync_AuctionHasEnded_UpdatesStatusToEnded()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = StatusActive,
//                start_time = now.AddHours(-2),
//                end_time = now.AddHours(-1),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal(StatusEnded, auction.status);
//        }

//        [Fact]
//        public async Task RunStatusUpdateForTestingAsync_EndedEnglishAuctionWithBids_UpdatesStatusToSold()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            var auction = new Auction
//            {
//                auction_id = 1,
//                status = StatusEnded,
//                start_time = now.AddHours(-2),
//                end_time = now.AddHours(-1),
//            };
//            db.Auctions.Add(auction);
//            db.Acceptances.Add(new Acceptance { auction_id = 1, accepted_price = 100 });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var resultAuction = await db.Auctions.FirstAsync();
//            Assert.Equal(StatusSold, resultAuction.status);
//        }

//        [Fact]
//        public async Task RunStatusUpdateForTestingAsync_EndedEnglishAuctionWithoutBids_UpdatesStatusToNotSold()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = StatusEnded,
//                start_time = now.AddHours(-2),
//                end_time = now.AddHours(-1),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal(StatusNotSold, auction.status);
//        }

//        [Fact]
//        public async Task RunStatusUpdateForTestingAsync_EndedDutchAuction_UpdatesStatusToEnded()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = StatusActive,
//                start_time = now.AddHours(-2),
//                end_time = now.AddHours(-1),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal(StatusEnded, auction.status);
//        }

//        #region UpdateStatusesAsync Tests - Status Transitions

//        [Fact]
//        public async Task UpdateStatusesAsync_AuctionInFuture_SetsStatusToUpcoming()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;
//            var futureStart = now.AddHours(2);
//            var futureEnd = now.AddHours(3);

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "active",
//                start_time = futureStart,
//                end_time = futureEnd,
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("upcoming", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_AuctionCurrentlyActive_SetsStatusToActive()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;
//            var pastStart = now.AddHours(-1);
//            var futureEnd = now.AddHours(1);

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "upcoming",
//                start_time = pastStart,
//                end_time = futureEnd,
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("active", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_AuctionPastEnd_SetsStatusToClosed()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;
//            var pastStart = now.AddHours(-3);
//            var pastEnd = now.AddHours(-1);

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "active",
//                start_time = pastStart,
//                end_time = pastEnd,
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("closed", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_MultipleAuctions_UpdatesAllCorrectly()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.AddRange(
//                new Auction
//                {
//                    auction_id = 1,
//                    status = "active",
//                    start_time = now.AddHours(1),
//                    end_time = now.AddHours(2),
//                },
//                new Auction
//                {
//                    auction_id = 2,
//                    status = "upcoming",
//                    start_time = now.AddHours(-1),
//                    end_time = now.AddHours(1),
//                },
//                new Auction
//                {
//                    auction_id = 3,
//                    status = "active",
//                    start_time = now.AddHours(-2),
//                    end_time = now.AddHours(-1),
//                }
//            );
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auctions = await db.Auctions.ToListAsync();
//            Assert.Equal("upcoming", auctions.First(a => a.auction_id == 1).status);
//            Assert.Equal("active", auctions.First(a => a.auction_id == 2).status);
//            Assert.Equal("closed", auctions.First(a => a.auction_id == 3).status);
//        }

//        #endregion

//        #region DateTime Kind Handling Tests

//        [Fact]
//        public async Task UpdateStatusesAsync_WithUtcDateTime_HandlesCorrectly()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var nowUtc = DateTime.UtcNow;
//            var futureUtc = nowUtc.AddHours(1);

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "active",
//                start_time = DateTime.SpecifyKind(futureUtc, DateTimeKind.Utc),
//                end_time = DateTime.SpecifyKind(futureUtc.AddHours(1), DateTimeKind.Utc),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, nowUtc);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("upcoming", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_WithLocalDateTime_ConvertsToUtcCorrectly()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var nowUtc = DateTime.UtcNow;
//            var futureLocal = nowUtc.AddHours(2).ToLocalTime();

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "active",
//                start_time = DateTime.SpecifyKind(futureLocal, DateTimeKind.Local),
//                end_time = DateTime.SpecifyKind(futureLocal.AddHours(1), DateTimeKind.Local),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, nowUtc);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("upcoming", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_WithUnspecifiedDateTime_TreatsAsUtc()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;
//            var futureStart = now.AddHours(1);

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "active",
//                start_time = DateTime.SpecifyKind(futureStart, DateTimeKind.Unspecified),
//                end_time = DateTime.SpecifyKind(futureStart.AddHours(1), DateTimeKind.Unspecified),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("upcoming", auction.status);
//        }

//        #endregion

//        #region Edge Cases and Boundary Tests

//        [Fact]
//        public async Task UpdateStatusesAsync_AuctionStartsExactlyAtNow_SetsStatusToActive()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "upcoming",
//                start_time = now,
//                end_time = now.AddHours(1),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("active", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_AuctionEndsExactlyAtNow_SetsStatusToClosed()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "active",
//                start_time = now.AddHours(-1),
//                end_time = now,
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("closed", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_WithDefaultEndTime_TreatsAsMaxValue()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;
//            var pastStart = now.AddHours(-1);

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "upcoming",
//                start_time = pastStart,
//                end_time = default,
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("active", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_WithNullStatus_UpdatesFromNull()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = null,
//                start_time = now.AddHours(1),
//                end_time = now.AddHours(2),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("upcoming", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_WithMixedCaseStatus_IgnoresCaseForComparison()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;
//            var futureStart = now.AddHours(1);

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "UPCOMING",
//                start_time = futureStart,
//                end_time = futureStart.AddHours(1),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            // Status should remain unchanged because it matches (case-insensitive)
//            Assert.Equal("UPCOMING", auction.status);
//        }

//        #endregion

//        #region Empty and Large Dataset Tests

//        [Fact]
//        public async Task UpdateStatusesAsync_WithNoAuctions_CompletsSuccessfully()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act & Assert - should not throw
//            await updater.RunStatusUpdateForTestingAsync(db, now);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_WithManyAuctions_UpdatesAllCorrectly()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            // Add 100 auctions with various statuses
//            for (int i = 0; i < 100; i++)
//            {
//                db.Auctions.Add(new Auction
//                {
//                    auction_id = i,
//                    status = "active",
//                    start_time = i % 3 == 0 ? now.AddHours(1) : now.AddHours(-1),
//                    end_time = i % 3 == 0 ? now.AddHours(2) : now.AddHours(1),
//                });
//            }
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auctions = await db.Auctions.ToListAsync();
//            Assert.Equal(100, auctions.Count);
            
//            // Verify some are upcoming and some are active
//            Assert.True(auctions.Any(a => a.status == "upcoming"));
//            Assert.True(auctions.Any(a => a.status == "active"));
//        }

//        #endregion

//        #region No Change Scenarios Tests

//        [Fact]
//        public async Task UpdateStatusesAsync_WhenStatusIsAlreadyCorrect_DoesNotUpdate()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.Add(new Auction
//            {
//                auction_id = 1,
//                status = "upcoming",
//                start_time = now.AddHours(1),
//                end_time = now.AddHours(2),
//            });
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auction = await db.Auctions.FirstAsync();
//            Assert.Equal("upcoming", auction.status);
//        }

//        [Fact]
//        public async Task UpdateStatusesAsync_WithAllAuctionsUpToDate_NoChangesRequired()
//        {
//            // Arrange
//            var (db, serviceProvider) = CreateInMemoryDbAndServiceProvider();
//            var now = DateTime.UtcNow;

//            db.Auctions.AddRange(
//                new Auction
//                {
//                    auction_id = 1,
//                    status = "upcoming",
//                    start_time = now.AddHours(1),
//                    end_time = now.AddHours(2),
//                },
//                new Auction
//                {
//                    auction_id = 2,
//                    status = "active",
//                    start_time = now.AddHours(-1),
//                    end_time = now.AddHours(1),
//                }
//            );
//            await db.SaveChangesAsync();

//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Act
//            await updater.RunStatusUpdateForTestingAsync(db, now);

//            // Assert
//            var auctions = await db.Auctions.ToListAsync();
//            Assert.Equal("upcoming", auctions.First(a => a.auction_id == 1).status);
//            Assert.Equal("active", auctions.First(a => a.auction_id == 2).status);
//        }

//        #endregion

//        #region Constructor Tests

//        [Fact]
//        public void Constructor_WithValidParameters_InitializesSuccessfully()
//        {
//            // Arrange
//            var serviceProvider = new ServiceCollection()
//                .AddDbContext<DBContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
//                .AddSingleton<ILogger<AuctionStatusUpdater>>(Mock.Of<ILogger<AuctionStatusUpdater>>())
//                .BuildServiceProvider();

//            // Act
//            var updater = new AuctionStatusUpdater(serviceProvider, Mock.Of<ILogger<AuctionStatusUpdater>>());

//            // Assert
//            Assert.NotNull(updater);
//        }

//        [Fact]
//        public void Constructor_WithNullProvider_ThrowsArgumentNullException()
//        {
//            // Act & Assert
//            Assert.Throws<ArgumentNullException>(() => 
//                new AuctionStatusUpdater(null, Mock.Of<ILogger<AuctionStatusUpdater>>())
//            );
//        }

//        [Fact]
//        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
//        {
//            // Arrange
//            var serviceProvider = new ServiceCollection()
//                .AddDbContext<DBContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
//                .BuildServiceProvider();

//            // Act & Assert
//            Assert.Throws<ArgumentNullException>(() => 
//                new AuctionStatusUpdater(serviceProvider, null)
//            );
//        }

//        #endregion
//    }
//}
