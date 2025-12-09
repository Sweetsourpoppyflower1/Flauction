using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flauction.Tests.ControllerTests
{
    public class AcceptancesControllerTests
    {
        private (DBContext, AcceptancesController) CreateContextAndController()
        {
            var services = new ServiceCollection();
            services.AddDbContext<DBContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<DBContext>();
            var controller = new AcceptancesController(dbContext);

            return (dbContext, controller);
        }

        #region GetAcceptances Tests

        [Fact]
        public async Task GetAcceptances_WithAcceptances_ReturnsAllAcceptances()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance1 = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };
            var acceptance2 = new Acceptance
            {
                acceptance_id = 2,
                auction_id = 2,
                company_id = "company-2",
                auction_lot_id = 2,
                tick_number = 2,
                accepted_price = 150m,
                accepted_quantity = 15,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance1);
            db.Acceptances.Add(acceptance2);
            await db.SaveChangesAsync();

            // Act
            var result = await controller.GetAcceptances();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Acceptance>>>(result);
            var returnValue = Assert.IsType<List<Acceptance>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAcceptances_WithNoAcceptances_ReturnsEmptyList()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            // Act
            var result = await controller.GetAcceptances();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Acceptance>>>(result);
            var returnValue = Assert.IsType<List<Acceptance>>(actionResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetAcceptances_WithMultipleAcceptances_ReturnsSortedByDatabase()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            for (int i = 1; i <= 5; i++)
            {
                db.Acceptances.Add(new Acceptance
                {
                    acceptance_id = i,
                    auction_id = i,
                    company_id = $"company-{i}",
                    auction_lot_id = i,
                    tick_number = i,
                    accepted_price = 100m * i,
                    accepted_quantity = 10 * i,
                    time = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();

            // Act
            var result = await controller.GetAcceptances();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Acceptance>>>(result);
            var returnValue = Assert.IsType<List<Acceptance>>(actionResult.Value);
            Assert.Equal(5, returnValue.Count);
            Assert.Equal(1, returnValue.First().acceptance_id);
            Assert.Equal(5, returnValue.Last().acceptance_id);
        }

        #endregion

        #region GetAcceptance Tests

        [Fact]
        public async Task GetAcceptance_WithValidId_ReturnsAcceptance()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance);
            await db.SaveChangesAsync();

            // Act
            var result = await controller.GetAcceptance(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Acceptance>>(result);
            var returnValue = Assert.IsType<Acceptance>(actionResult.Value);
            Assert.Equal(1, returnValue.acceptance_id);
            Assert.Equal("company-1", returnValue.company_id);
        }

        [Fact]
        public async Task GetAcceptance_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            // Act
            var result = await controller.GetAcceptance(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAcceptance_WithValidId_ReturnsCorrectData()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 5,
                auction_id = 10,
                company_id = "company-abc",
                auction_lot_id = 3,
                tick_number = 5,
                accepted_price = 250.50m,
                accepted_quantity = 25,
                time = DateTime.UtcNow.AddHours(-2)
            };

            db.Acceptances.Add(acceptance);
            await db.SaveChangesAsync();

            // Act
            var result = await controller.GetAcceptance(5);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Acceptance>>(result);
            var returnValue = Assert.IsType<Acceptance>(actionResult.Value);
            Assert.Equal(5, returnValue.acceptance_id);
            Assert.Equal(10, returnValue.auction_id);
            Assert.Equal("company-abc", returnValue.company_id);
            Assert.Equal(3, returnValue.auction_lot_id);
            Assert.Equal(5, returnValue.tick_number);
            Assert.Equal(250.50m, returnValue.accepted_price);
            Assert.Equal(25, returnValue.accepted_quantity);
        }

        [Fact]
        public async Task GetAcceptance_WithZeroId_ReturnsNotFound()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            // Act
            var result = await controller.GetAcceptance(0);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAcceptance_WithNegativeId_ReturnsNotFound()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            // Act
            var result = await controller.GetAcceptance(-1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region PostAcceptance Tests

        [Fact]
        public async Task PostAcceptance_WithValidAcceptance_ReturnsCreatedAtAction()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            // Act
            var result = await controller.PostAcceptance(acceptance);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(controller.GetAcceptance), actionResult.ActionName);
            Assert.Equal(acceptance.acceptance_id, ((Acceptance)actionResult.Value).acceptance_id);
        }

        [Fact]
        public async Task PostAcceptance_WithValidAcceptance_SavesToDatabase()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            // Act
            await controller.PostAcceptance(acceptance);

            // Assert
            var savedAcceptance = await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
            Assert.Equal("company-1", savedAcceptance.company_id);
            Assert.Equal(100m, savedAcceptance.accepted_price);
        }

        [Fact]
        public async Task PostAcceptance_WithMultipleAcceptances_SavesAll()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance1 = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };
            var acceptance2 = new Acceptance
            {
                acceptance_id = 2,
                auction_id = 2,
                company_id = "company-2",
                auction_lot_id = 2,
                tick_number = 2,
                accepted_price = 150m,
                accepted_quantity = 15,
                time = DateTime.UtcNow
            };

            // Act
            await controller.PostAcceptance(acceptance1);
            await controller.PostAcceptance(acceptance2);

            // Assert
            Assert.Equal(2, await db.Acceptances.CountAsync());
        }

        [Fact]
        public async Task PostAcceptance_WithHighPrice_SavesCorrectly()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 99999.99m,
                accepted_quantity = 1000,
                time = DateTime.UtcNow
            };

            // Act
            await controller.PostAcceptance(acceptance);

            // Assert
            var savedAcceptance = await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
            Assert.Equal(99999.99m, savedAcceptance.accepted_price);
        }

        #endregion

        #region PutAcceptance Tests

        [Fact]
        public async Task PutAcceptance_WithValidIdAndAcceptance_ReturnsNoContent()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance);
            await db.SaveChangesAsync();

            var updatedAcceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 2,
                accepted_price = 150m,
                accepted_quantity = 15,
                time = DateTime.UtcNow
            };

            // Act
            var result = await controller.PutAcceptance(1, updatedAcceptance);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutAcceptance_WithValidIdAndAcceptance_UpdatesDatabase()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance);
            await db.SaveChangesAsync();

            var updatedAcceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 2,
                accepted_price = 150m,
                accepted_quantity = 15,
                time = DateTime.UtcNow
            };

            // Act
            await controller.PutAcceptance(1, updatedAcceptance);

            // Assert
            var savedAcceptance = await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 1);
            Assert.NotNull(savedAcceptance);
            Assert.Equal(2, savedAcceptance.tick_number);
            Assert.Equal(150m, savedAcceptance.accepted_price);
            Assert.Equal(15, savedAcceptance.accepted_quantity);
        }

        [Fact]
        public async Task PutAcceptance_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            // Act
            var result = await controller.PutAcceptance(2, acceptance);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutAcceptance_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 999,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            // Act
            var result = await controller.PutAcceptance(999, acceptance);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutAcceptance_UpdatesOnlySpecificAcceptance()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance1 = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };
            var acceptance2 = new Acceptance
            {
                acceptance_id = 2,
                auction_id = 2,
                company_id = "company-2",
                auction_lot_id = 2,
                tick_number = 2,
                accepted_price = 200m,
                accepted_quantity = 20,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance1);
            db.Acceptances.Add(acceptance2);
            await db.SaveChangesAsync();

            var updatedAcceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1-updated",
                auction_lot_id = 1,
                tick_number = 5,
                accepted_price = 500m,
                accepted_quantity = 50,
                time = DateTime.UtcNow
            };

            // Act
            await controller.PutAcceptance(1, updatedAcceptance);

            // Assert
            var updated = await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 1);
            var unchanged = await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 2);

            Assert.Equal("company-1-updated", updated.company_id);
            Assert.Equal(500m, updated.accepted_price);
            Assert.Equal("company-2", unchanged.company_id);
            Assert.Equal(200m, unchanged.accepted_price);
        }

        #endregion

        #region DeleteAcceptance Tests

        [Fact]
        public async Task DeleteAcceptance_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance);
            await db.SaveChangesAsync();

            // Act
            var result = await controller.DeleteAcceptance(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAcceptance_WithValidId_RemovesFromDatabase()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance);
            await db.SaveChangesAsync();

            // Act
            await controller.DeleteAcceptance(1);

            // Assert
            var deletedAcceptance = await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 1);
            Assert.Null(deletedAcceptance);
        }

        [Fact]
        public async Task DeleteAcceptance_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            // Act
            var result = await controller.DeleteAcceptance(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAcceptance_DeletesOnlySpecificAcceptance()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance1 = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };
            var acceptance2 = new Acceptance
            {
                acceptance_id = 2,
                auction_id = 2,
                company_id = "company-2",
                auction_lot_id = 2,
                tick_number = 2,
                accepted_price = 200m,
                accepted_quantity = 20,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance1);
            db.Acceptances.Add(acceptance2);
            await db.SaveChangesAsync();

            // Act
            await controller.DeleteAcceptance(1);

            // Assert
            Assert.Null(await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 1));
            Assert.NotNull(await db.Acceptances.FirstOrDefaultAsync(a => a.acceptance_id == 2));
            Assert.Equal(1, await db.Acceptances.CountAsync());
        }

        [Fact]
        public async Task DeleteAcceptance_WithZeroId_ReturnsNotFound()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            // Act
            var result = await controller.DeleteAcceptance(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region AcceptanceExists Tests

        [Fact]
        public void AcceptanceExists_WithExistingId_ReturnsTrue()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            db.Acceptances.Add(acceptance);
            db.SaveChanges();

            // Act - Use reflection to call private method
            var method = typeof(AcceptancesController).GetMethod("AcceptanceExists", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)method.Invoke(controller, new object[] { 1 });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AcceptanceExists_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();

            // Act - Use reflection to call private method
            var method = typeof(AcceptancesController).GetMethod("AcceptanceExists",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)method.Invoke(controller, new object[] { 999 });

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task FullCRUDCycle_CreateReadUpdateDelete_WorksCorrectly()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptance = new Acceptance
            {
                acceptance_id = 1,
                auction_id = 1,
                company_id = "company-1",
                auction_lot_id = 1,
                tick_number = 1,
                accepted_price = 100m,
                accepted_quantity = 10,
                time = DateTime.UtcNow
            };

            // Act - Create
            await controller.PostAcceptance(acceptance);
            var getResult = await controller.GetAcceptance(1);
            var retrieved = ((ActionResult<Acceptance>)getResult).Value;

            // Update
            retrieved.accepted_price = 150m;
            await controller.PutAcceptance(1, retrieved);

            // Read updated
            var getUpdatedResult = await controller.GetAcceptance(1);
            var updated = ((ActionResult<Acceptance>)getUpdatedResult).Value;

            // Delete
            await controller.DeleteAcceptance(1);
            var getFinalResult = await controller.GetAcceptance(1);

            // Assert
            Assert.Equal(100m, retrieved.accepted_price);
            Assert.Equal(150m, updated.accepted_price);
            Assert.IsType<NotFoundResult>(getFinalResult.Result);
        }

        [Fact]
        public async Task MultipleAcceptances_ComplexScenario_AllOperationsWork()
        {
            // Arrange
            var (db, controller) = CreateContextAndController();
            var acceptances = Enumerable.Range(1, 10).Select(i => new Acceptance
            {
                acceptance_id = i,
                auction_id = i,
                company_id = $"company-{i}",
                auction_lot_id = i,
                tick_number = i,
                accepted_price = 100m * i,
                accepted_quantity = 10 * i,
                time = DateTime.UtcNow
            }).ToList();

            // Act - Create all
            foreach (var acceptance in acceptances)
            {
                await controller.PostAcceptance(acceptance);
            }

            // Get all
            var getAllResult = await controller.GetAcceptances();
            var allAcceptances = ((ActionResult<IEnumerable<Acceptance>>)getAllResult).Value.ToList();

            // Update some
            for (int i = 1; i <= 5; i++)
            {
                var toUpdate = allAcceptances[i - 1];
                toUpdate.accepted_price = 999m;
                await controller.PutAcceptance(i, toUpdate);
            }

            // Delete some
            await controller.DeleteAcceptance(1);
            await controller.DeleteAcceptance(2);

            // Get all again
            var finalResult = await controller.GetAcceptances();
            var finalAcceptances = ((ActionResult<IEnumerable<Acceptance>>)finalResult).Value.ToList();

            // Assert
            Assert.Equal(10, allAcceptances.Count);
            Assert.Equal(8, finalAcceptances.Count);
            Assert.All(finalAcceptances.Where(a => a.acceptance_id <= 5), 
                a => Assert.Equal(999m, a.accepted_price));
        }

        #endregion
    }
}