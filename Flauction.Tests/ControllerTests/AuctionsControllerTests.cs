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
    public class AuctionsControllerTests
    {
        private DbContextOptions<DBContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAuctions_ReturnsAllAuctions()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Auctions.AddRange(
                    new Auction { auction_id = 1, status = "Active" },
                    new Auction { auction_id = 2, status = "Upcoming" }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionsController(context);

                // Act
                var result = await controller.GetAuctions();

                // Assert
                var auctions = Assert.IsAssignableFrom<IEnumerable<Auction>>(result.Value);
                Assert.Equal(2, auctions.Count());
            }
        }

        [Fact]
        public async Task GetAuction_WithValidId_ReturnsAuction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testAuction = new Auction { auction_id = 1, status = "Active" };
            using (var context = new DBContext(options))
            {
                context.Auctions.Add(testAuction);
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionsController(context);

                // Act
                var result = await controller.GetAuction(1);

                // Assert
                var actionResult = Assert.IsType<ActionResult<Auction>>(result);
                var auction = Assert.IsType<Auction>(actionResult.Value);
                Assert.Equal(1, auction.auction_id);
                Assert.Equal("Active", auction.status);
            }
        }

        [Fact]
        public async Task GetAuction_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DBContext(options);
            var controller = new AuctionsController(context);

            // Act
            var result = await controller.GetAuction(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostAuction_CreatesAndReturnsAuction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var newAuction = new Auction { auction_id = 1, status = "New" };
            
            using (var context = new DBContext(options))
            {
                var controller = new AuctionsController(context);

                // Act
                var result = await controller.PostAuction(newAuction);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var auction = Assert.IsType<Auction>(createdAtActionResult.Value);
                Assert.Equal("New", auction.status);
                Assert.Equal(1, auction.auction_id);
            }

            // Verify it was saved to the database
            using (var context = new DBContext(options))
            {
                Assert.Equal(1, await context.Auctions.CountAsync());
                Assert.Equal("New", (await context.Auctions.FirstAsync()).status);
            }
        }

        [Fact]
        public async Task PutAuction_WithValidId_UpdatesAuction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Auctions.Add(new Auction { auction_id = 1, status = "Initial" });
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionsController(context);
                var updatedAuction = new Auction { auction_id = 1, status = "Updated" };

                // Act
                var result = await controller.PutAuction(1, updatedAuction);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify the update in the database
            using (var context = new DBContext(options))
            {
                var auctionInDb = await context.Auctions.FindAsync(1);
                Assert.NotNull(auctionInDb);
                Assert.Equal("Updated", auctionInDb.status);
            }
        }

        [Fact]
        public async Task DeleteAuction_WithValidId_RemovesAuction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Auctions.Add(new Auction { auction_id = 1, status = "ToDelete" });
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new AuctionsController(context);

                // Act
                var result = await controller.DeleteAuction(1);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify it was removed from the database
            using (var context = new DBContext(options))
            {
                Assert.Equal(0, await context.Auctions.CountAsync());
            }
        }
    }
}