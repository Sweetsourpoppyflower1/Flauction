using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Flauction.Tests.ControllerTests
{
    public class AuctionLotsControllerTests
    {
        private DbContextOptions<DBContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private AuctionLot CreateTestAuctionLot(int id = 1)
        {
            return new AuctionLot
            {
                auctionlot_id = id,
                plant_id = 1,
                unit_per_container = 10,
                containers_in_lot = 5,
                min_pickup = 2,
                start_quantity = 50,
                remaining_quantity = 50
            };
        }

        [Fact]
        public async Task GetAuctionLots_ReturnsAllAuctionLots()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.AddRange(
                    CreateTestAuctionLot(1),
                    CreateTestAuctionLot(2)
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act
                var result = await controller.GetAuctionLots();

                // Assert
                var auctionLots = Assert.IsAssignableFrom<IEnumerable<AuctionLot>>(result.Value);
                Assert.Equal(2, auctionLots.Count());
            }
        }

        [Fact]
        public async Task GetAuctionLots_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DBContext(options);
            var controller = new AuctionLotsController(context);

            // Act
            var result = await controller.GetAuctionLots();

            // Assert
            var auctionLots = Assert.IsAssignableFrom<IEnumerable<AuctionLot>>(result.Value);
            Assert.Empty(auctionLots);
        }

        [Fact]
        public async Task GetAuctionLot_WithValidId_ReturnsAuctionLot()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testAuctionLot = CreateTestAuctionLot(1);
            using (var context = new DBContext(options))
            {
                context.AuctionLots.Add(testAuctionLot);
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act
                var result = await controller.GetAuctionLot(1);

                // Assert
                var actionResult = Assert.IsType<ActionResult<AuctionLot>>(result);
                var auctionLot = Assert.IsType<AuctionLot>(actionResult.Value);
                Assert.Equal(1, auctionLot.auctionlot_id);
                Assert.Equal(1, auctionLot.plant_id);
                Assert.Equal(50, auctionLot.start_quantity);
            }
        }

        [Fact]
        public async Task GetAuctionLot_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DBContext(options);
            var controller = new AuctionLotsController(context);

            // Act
            var result = await controller.GetAuctionLot(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostAuctionLot_CreatesAndReturnsAuctionLot()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var newAuctionLot = CreateTestAuctionLot(1);

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act
                var result = await controller.PostAuctionLot(newAuctionLot);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal(nameof(AuctionLotsController.GetAuctionLot), createdAtActionResult.ActionName);
                var returnedAuctionLot = Assert.IsType<AuctionLot>(createdAtActionResult.Value);
                Assert.Equal(1, returnedAuctionLot.auctionlot_id);
                Assert.Equal(50, returnedAuctionLot.start_quantity);
            }

            // Verify it was saved to the database
            using (var context = new DBContext(options))
            {
                Assert.Equal(1, await context.AuctionLots.CountAsync());
                var savedLot = await context.AuctionLots.FirstAsync();
                Assert.Equal(50, savedLot.start_quantity);
            }
        }

        [Fact]
        public async Task PostAuctionLot_WithMultipleAuctionLots_SavesAll()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var lot1 = CreateTestAuctionLot(1);
            var lot2 = CreateTestAuctionLot(2);

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act
                await controller.PostAuctionLot(lot1);
                await controller.PostAuctionLot(lot2);

                // Assert
                var lotsInDb = await context.AuctionLots.ToListAsync();
                Assert.Equal(2, lotsInDb.Count);
            }
        }

        [Fact]
        public async Task PutAuctionLot_WithValidId_UpdatesAuctionLot()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.Add(CreateTestAuctionLot(1));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);
                var updatedAuctionLot = CreateTestAuctionLot(1);
                updatedAuctionLot.remaining_quantity = 25;
                updatedAuctionLot.start_quantity = 25;

                // Act
                var result = await controller.PutAuctionLot(1, updatedAuctionLot);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify the update in the database
            using (var context = new DBContext(options))
            {
                var auctionLotInDb = await context.AuctionLots.FindAsync(1);
                Assert.NotNull(auctionLotInDb);
                Assert.Equal(25, auctionLotInDb.remaining_quantity);
                Assert.Equal(25, auctionLotInDb.start_quantity);
            }
        }

        [Fact]
        public async Task PutAuctionLot_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.Add(CreateTestAuctionLot(1));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);
                var auctionLot = CreateTestAuctionLot(2);

                // Act
                var result = await controller.PutAuctionLot(1, auctionLot);

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }

        [Fact]
        public async Task PutAuctionLot_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.Add(CreateTestAuctionLot(1));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);
                var auctionLot = CreateTestAuctionLot(999);

                // Act
                var result = await controller.PutAuctionLot(999, auctionLot);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteAuctionLot_WithValidId_RemovesAuctionLot()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.Add(CreateTestAuctionLot(1));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act
                var result = await controller.DeleteAuctionLot(1);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify it was removed from the database
            using (var context = new DBContext(options))
            {
                Assert.Equal(0, await context.AuctionLots.CountAsync());
            }
        }

        [Fact]
        public async Task DeleteAuctionLot_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DBContext(options);
            var controller = new AuctionLotsController(context);

            // Act
            var result = await controller.DeleteAuctionLot(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAuctionLot_WithMultipleLots_RemovesOnlyTarget()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.AddRange(
                    CreateTestAuctionLot(1),
                    CreateTestAuctionLot(2),
                    CreateTestAuctionLot(3)
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act
                var result = await controller.DeleteAuctionLot(2);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify only the target was removed
            using (var context = new DBContext(options))
            {
                Assert.Equal(2, await context.AuctionLots.CountAsync());
                Assert.Null(await context.AuctionLots.FindAsync(2));
                Assert.NotNull(await context.AuctionLots.FindAsync(1));
                Assert.NotNull(await context.AuctionLots.FindAsync(3));
            }
        }

        [Fact]
        public async Task AuctionLotExists_WithExistingId_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.Add(CreateTestAuctionLot(1));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act - AuctionLotExists is private, so we test it indirectly through PutAuctionLot
                var result = await controller.PutAuctionLot(1, CreateTestAuctionLot(1));

                // Assert - If AuctionLotExists returned false, PutAuctionLot would return NotFound on concurrency exception
                Assert.IsType<NoContentResult>(result);
            }
        }

        [Fact]
        public async Task AuctionLotExists_WithNonexistentId_ReturnsFalse()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.AuctionLots.Add(CreateTestAuctionLot(1));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionLotsController(context);

                // Act - Testing AuctionLotExists indirectly through PutAuctionLot
                var result = await controller.PutAuctionLot(999, CreateTestAuctionLot(999));

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }
    }
}