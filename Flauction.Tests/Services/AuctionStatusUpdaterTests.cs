using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flauction.Data;
using Flauction.Models;
using Flauction.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flauction.Tests.Services
{
    public class AuctionStatusUpdaterTests : IAsyncLifetime
    {
        private readonly Mock<ILogger<AuctionStatusUpdater>> _mockLogger;

        public AuctionStatusUpdaterTests()
        {
            _mockLogger = new Mock<ILogger<AuctionStatusUpdater>>();
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public Task DisposeAsync() => Task.CompletedTask;

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullProvider_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new AuctionStatusUpdater(null!, _mockLogger.Object));
            Assert.Equal("provider", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            var mockProvider = new Mock<IServiceProvider>();
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new AuctionStatusUpdater(mockProvider.Object, null!));
            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParameters_Succeeds()
        {
            var mockProvider = new Mock<IServiceProvider>();
            var updater = new AuctionStatusUpdater(mockProvider.Object, _mockLogger.Object);
            Assert.NotNull(updater);
        }

        #endregion

        #region DetermineStatus Tests

        [Theory]
        [InlineData(10, 12, 13)]
        [InlineData(9, 12, 13)]
        [InlineData(11, 12, 13)]
        public void DetermineStatus_WhenNowBeforeStart_ReturnsUpcoming(int nowHour, int startHour, int endHour)
        {
            var baseDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var now = baseDate.AddHours(nowHour);
            var startTime = baseDate.AddHours(startHour);
            var endTime = baseDate.AddHours(endHour);

            var status = InvokeDetermineStatus(now, startTime, endTime);

            Assert.Equal("upcoming", status);
        }

        [Theory]
        [InlineData(12, 12, 13)]
        [InlineData(12.5, 12, 13)]
        [InlineData(12.99, 12, 13)]
        public void DetermineStatus_WhenNowBetweenStartAndEnd_ReturnsActive(double nowHour, int startHour, int endHour)
        {
            var baseDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var now = baseDate.AddHours(nowHour);
            var startTime = baseDate.AddHours(startHour);
            var endTime = baseDate.AddHours(endHour);

            var status = InvokeDetermineStatus(now, startTime, endTime);

            Assert.Equal("active", status);
        }

        [Theory]
        [InlineData(13, 12, 13)]
        [InlineData(14, 12, 13)]
        [InlineData(23, 12, 13)]
        public void DetermineStatus_WhenNowAtOrAfterEnd_ReturnsCompleted(int nowHour, int startHour, int endHour)
        {
            var baseDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var now = baseDate.AddHours(nowHour);
            var startTime = baseDate.AddHours(startHour);
            var endTime = baseDate.AddHours(endHour);

            var status = InvokeDetermineStatus(now, startTime, endTime);

            Assert.Equal("completed", status);
        }

        #endregion

        #region UpdateStatusesAsync Tests (via integration)

        [Fact]
        public async Task UpdateStatusesAsync_WithNoAuctions_CompletesSuccessfully()
        {
            var dbOptions = CreateDbContextOptions("UpdateAsync_NoAuctions");
            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
            }

            var mockServiceProvider = CreateMockServiceProvider(dbOptions);
            var updater = new AuctionStatusUpdater(mockServiceProvider.Object, _mockLogger.Object);

            await InvokeUpdateStatusesAsync(updater);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateStatusesAsync_WithUpcomingAuction_UpdatesStatus()
        {
            var dbOptions = CreateDbContextOptions("UpdateAsync_Upcoming");
            var startTime = DateTime.UtcNow.AddHours(2);

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.Add(new Auction
                {
                    auction_id = 1,
                    auctionmaster_id = "master-1",
                    plant_id = 1,
                    status = "completed",
                    start_time = startTime,
                    duration_minutes = 60
                });
                await context.SaveChangesAsync();
            }

            var mockServiceProvider = CreateMockServiceProvider(dbOptions);
            var updater = new AuctionStatusUpdater(mockServiceProvider.Object, _mockLogger.Object);

            await InvokeUpdateStatusesAsync(updater);

            using (var context = new DBContext(dbOptions))
            {
                var auction = context.Auctions.First();
                Assert.Equal("upcoming", auction.status);
            }
        }

        [Fact]
        public async Task UpdateStatusesAsync_WithActiveAuction_DoesNotUpdateCompleted()
        {
            var dbOptions = CreateDbContextOptions("UpdateAsync_Active");
            var startTime = DateTime.UtcNow.AddHours(-1);

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.Add(new Auction
                {
                    auction_id = 1,
                    auctionmaster_id = "master-1",
                    plant_id = 1,
                    status = "completed",
                    start_time = startTime,
                    duration_minutes = 30
                });
                await context.SaveChangesAsync();
            }

            var mockServiceProvider = CreateMockServiceProvider(dbOptions);
            var updater = new AuctionStatusUpdater(mockServiceProvider.Object, _mockLogger.Object);

            await InvokeUpdateStatusesAsync(updater);

            using (var context = new DBContext(dbOptions))
            {
                var auction = context.Auctions.First();
                Assert.Equal("completed", auction.status);
            }
        }

        [Fact]
        public async Task UpdateStatusesAsync_SkipsCompletedAuctions()
        {
            var dbOptions = CreateDbContextOptions("UpdateAsync_SkipsCompleted");

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.AddRange(
                    new Auction
                    {
                        auction_id = 1,
                        auctionmaster_id = "master-1",
                        plant_id = 1,
                        status = "upcoming",
                        start_time = DateTime.UtcNow.AddHours(1),
                        duration_minutes = 60
                    },
                    new Auction
                    {
                        auction_id = 2,
                        auctionmaster_id = "master-2",
                        plant_id = 2,
                        status = "completed",
                        start_time = DateTime.UtcNow.AddHours(-2),
                        duration_minutes = 60
                    }
                );
                await context.SaveChangesAsync();
            }

            var mockServiceProvider = CreateMockServiceProvider(dbOptions);
            var updater = new AuctionStatusUpdater(mockServiceProvider.Object, _mockLogger.Object);

            await InvokeUpdateStatusesAsync(updater);

            using (var context = new DBContext(dbOptions))
            {
                var completed = context.Auctions.First(a => a.auction_id == 2);
                Assert.Equal("completed", completed.status);
            }
        }

        [Fact]
        public async Task UpdateStatusesAsync_UpdatesMultipleAuctions()
        {
            var dbOptions = CreateDbContextOptions("UpdateAsync_Multiple");
            var now = DateTime.UtcNow;

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.AddRange(
                    new Auction
                    {
                        auction_id = 1,
                        auctionmaster_id = "master-1",
                        plant_id = 1,
                        status = "completed",
                        start_time = now.AddHours(1),
                        duration_minutes = 60
                    },
                    new Auction
                    {
                        auction_id = 2,
                        auctionmaster_id = "master-2",
                        plant_id = 2,
                        status = "upcoming",
                        start_time = now.AddHours(-1),
                        duration_minutes = 120
                    }
                );
                await context.SaveChangesAsync();
            }

            var mockServiceProvider = CreateMockServiceProvider(dbOptions);
            var updater = new AuctionStatusUpdater(mockServiceProvider.Object, _mockLogger.Object);

            await InvokeUpdateStatusesAsync(updater);

            using (var context = new DBContext(dbOptions))
            {
                var auctions = context.Auctions.OrderBy(a => a.auction_id).ToList();
                Assert.Equal("upcoming", auctions[0].status);
                Assert.Equal("active", auctions[1].status);
            }
        }

        #endregion

        #region RunStatusUpdateForTestingAsync Tests

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_WithEmptyDatabase_CompletesSuccessfully()
        {
            var dbOptions = CreateDbContextOptions("Testing_Empty");

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                var mockProvider = new Mock<IServiceProvider>();
                var updater = new AuctionStatusUpdater(mockProvider.Object, _mockLogger.Object);

                await updater.RunStatusUpdateForTestingAsync(context, DateTime.UtcNow);

                Assert.Empty(context.Auctions);
            }
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_UpdatesStatusesToUpcoming()
        {
            var dbOptions = CreateDbContextOptions("Testing_Upcoming");
            var testNow = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
            var startTime = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.Add(new Auction
                {
                    auction_id = 1,
                    auctionmaster_id = "test-master",
                    plant_id = 1,
                    status = "active",
                    start_time = startTime,
                    duration_minutes = 60
                });
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(dbOptions))
            {
                var mockProvider = new Mock<IServiceProvider>();
                var updater = new AuctionStatusUpdater(mockProvider.Object, _mockLogger.Object);

                await updater.RunStatusUpdateForTestingAsync(context, testNow);

                var updatedAuction = context.Auctions.First();
                Assert.Equal("upcoming", updatedAuction.status);
            }
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_UpdatesStatusesToActive()
        {
            var dbOptions = CreateDbContextOptions("Testing_Active");
            var testNow = new DateTime(2026, 1, 15, 12, 30, 0, DateTimeKind.Utc);
            var startTime = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.Add(new Auction
                {
                    auction_id = 1,
                    auctionmaster_id = "test-master",
                    plant_id = 1,
                    status = "upcoming",
                    start_time = startTime,
                    duration_minutes = 60
                });
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(dbOptions))
            {
                var mockProvider = new Mock<IServiceProvider>();
                var updater = new AuctionStatusUpdater(mockProvider.Object, _mockLogger.Object);

                await updater.RunStatusUpdateForTestingAsync(context, testNow);

                var updatedAuction = context.Auctions.First();
                Assert.Equal("active", updatedAuction.status);
            }
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_UpdatesStatusesToCompleted()
        {
            var dbOptions = CreateDbContextOptions("Testing_Completed");
            var testNow = new DateTime(2026, 1, 15, 14, 0, 0, DateTimeKind.Utc);
            var startTime = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.Add(new Auction
                {
                    auction_id = 1,
                    auctionmaster_id = "test-master",
                    plant_id = 1,
                    status = "active",
                    start_time = startTime,
                    duration_minutes = 60
                });
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(dbOptions))
            {
                var mockProvider = new Mock<IServiceProvider>();
                var updater = new AuctionStatusUpdater(mockProvider.Object, _mockLogger.Object);

                await updater.RunStatusUpdateForTestingAsync(context, testNow);

                var updatedAuction = context.Auctions.First();
                Assert.Equal("completed", updatedAuction.status);
            }
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_WithMultipleAuctions_UpdatesAll()
        {
            var dbOptions = CreateDbContextOptions("Testing_Multiple");
            var testNow = new DateTime(2026, 1, 15, 12, 30, 0, DateTimeKind.Utc);

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.AddRange(
                    new Auction
                    {
                        auction_id = 1,
                        auctionmaster_id = "master-1",
                        plant_id = 1,
                        status = "upcoming",
                        start_time = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                        duration_minutes = 60
                    },
                    new Auction
                    {
                        auction_id = 2,
                        auctionmaster_id = "master-2",
                        plant_id = 2,
                        status = "active",
                        start_time = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc),
                        duration_minutes = 60
                    },
                    new Auction
                    {
                        auction_id = 3,
                        auctionmaster_id = "master-3",
                        plant_id = 3,
                        status = "active",
                        start_time = new DateTime(2026, 1, 15, 11, 0, 0, DateTimeKind.Utc),
                        duration_minutes = 30
                    }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(dbOptions))
            {
                var mockProvider = new Mock<IServiceProvider>();
                var updater = new AuctionStatusUpdater(mockProvider.Object, _mockLogger.Object);

                await updater.RunStatusUpdateForTestingAsync(context, testNow);

                var auctions = context.Auctions.OrderBy(a => a.auction_id).ToList();
                Assert.Equal("active", auctions[0].status);
                Assert.Equal("active", auctions[1].status);
                Assert.Equal("completed", auctions[2].status);
            }
        }

        [Fact]
        public async Task RunStatusUpdateForTestingAsync_PreservesAuctionIds()
        {
            var dbOptions = CreateDbContextOptions("Testing_PreserveIds");
            var testNow = DateTime.UtcNow;

            using (var context = new DBContext(dbOptions))
            {
                context.Database.EnsureCreated();
                context.Auctions.AddRange(
                    new Auction
                    {
                        auction_id = 42,
                        auctionmaster_id = "master-1",
                        plant_id = 1,
                        status = "active",
                        start_time = testNow,
                        duration_minutes = 60
                    },
                    new Auction
                    {
                        auction_id = 99,
                        auctionmaster_id = "master-2",
                        plant_id = 2,
                        status = "active",
                        start_time = testNow,
                        duration_minutes = 60
                    }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(dbOptions))
            {
                var mockProvider = new Mock<IServiceProvider>();
                var updater = new AuctionStatusUpdater(mockProvider.Object, _mockLogger.Object);

                await updater.RunStatusUpdateForTestingAsync(context, testNow);

                var auctionIds = context.Auctions.Select(a => a.auction_id).OrderBy(id => id).ToList();
                Assert.Equal(new[] { 42, 99 }, auctionIds);
            }
        }

        #endregion

        #region Helper Methods

        private DbContextOptions<DBContext> CreateDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName + "_" + Guid.NewGuid())
                .Options;
        }

        private Mock<IServiceProvider> CreateMockServiceProvider(DbContextOptions<DBContext> dbOptions)
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            var mockServiceScope = new Mock<IServiceScope>();

            mockServiceProvider
                .Setup(x => x.CreateScope())
                .Returns(mockServiceScope.Object);

            mockServiceScope
                .Setup(x => x.ServiceProvider.GetRequiredService<DBContext>())
                .Returns(new DBContext(dbOptions));

            return mockServiceProvider;
        }

        private string InvokeDetermineStatus(DateTime now, DateTime startTime, DateTime endTime)
        {
            var method = typeof(AuctionStatusUpdater).GetMethod(
                "DetermineStatus",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (method == null)
                throw new InvalidOperationException("DetermineStatus method not found");

            var result = method.Invoke(null, new object[] { now, startTime, endTime });
            return result as string ?? throw new InvalidOperationException("DetermineStatus returned null");
        }

        private async Task InvokeUpdateStatusesAsync(AuctionStatusUpdater updater)
        {
            var method = typeof(AuctionStatusUpdater).GetMethod(
                "UpdateStatusesAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method == null)
                throw new InvalidOperationException("UpdateStatusesAsync method not found");

            var result = method.Invoke(updater, new object[] { CancellationToken.None });
            if (result is Task task)
            {
                await task;
            }
        }

        #endregion
    }
}