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
    public class PlantsControllerTests
    {
        private DbContextOptions<DBContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private Plant CreateTestPlant(int id = 1, string supplierId = "supplier1", string productName = "Rose",
            string category = "Flowers", string form = "Stem", string quality = "Premium",
            string minStem = "50cm", string stemsBunch = "10", string maturity = "Open",
            string desc = "Beautiful roses", int startPrice = 100, int minPrice = 50)
        {
            return new Plant
            {
                plant_id = id,
                supplier_id = supplierId,
                productname = productName,
                category = category,
                form = form,
                quality = quality,
                min_stem = minStem,
                stems_bunch = stemsBunch,
                maturity = maturity,
                desc = desc,
                start_price = startPrice,
                min_price = minPrice
            };
        }

        #region GetPlants Tests

        [Fact]
        public async Task GetPlants_ReturnsAllPlants()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose"),
                    CreateTestPlant(2, "supplier2", "Tulip"),
                    CreateTestPlant(3, "supplier1", "Sunflower")
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlants();

                // Assert
                var plants = Assert.IsAssignableFrom<IEnumerable<Plant>>(result.Value);
                Assert.Equal(3, plants.Count());
            }
        }

        [Fact]
        public async Task GetPlants_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var options = CreateNewContextOptions();

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlants();

                // Assert
                var plants = Assert.IsAssignableFrom<IEnumerable<Plant>>(result.Value);
                Assert.Empty(plants);
            }
        }

        [Fact]
        public async Task GetPlants_WithMultiplePlants_ReturnsAllWithCorrectData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose", "Flowers", "Stem", "Premium", "50cm", "10", "Open", "Beautiful roses", 100, 50),
                    CreateTestPlant(2, "supplier2", "Tulip", "Flowers", "Stem", "Standard", "40cm", "5", "Bud", "Lovely tulips", 80, 40),
                    CreateTestPlant(3, "supplier1", "Sunflower", "Flowers", "Bundle", "Premium", "60cm", "8", "Open", "Bright sunflowers", 120, 60)
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlants();

                // Assert
                var plants = result.Value.ToList();
                Assert.Equal(3, plants.Count);
                Assert.Contains(plants, p => p.productname == "Rose" && p.supplier_id == "supplier1");
                Assert.Contains(plants, p => p.productname == "Tulip" && p.start_price == 80);
                Assert.Contains(plants, p => p.productname == "Sunflower" && p.plant_id == 3);
            }
        }

        [Fact]
        public async Task GetPlants_WithDifferentSuppliers_ReturnsAll()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose"),
                    CreateTestPlant(2, "supplier2", "Tulip"),
                    CreateTestPlant(3, "supplier3", "Daisy")
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlants();

                // Assert
                var plants = result.Value.ToList();
                Assert.Equal(3, plants.Count);
                Assert.Single(plants.Where(p => p.supplier_id == "supplier1"));
                Assert.Single(plants.Where(p => p.supplier_id == "supplier2"));
                Assert.Single(plants.Where(p => p.supplier_id == "supplier3"));
            }
        }

        #endregion

        #region GetPlant Tests

        [Fact]
        public async Task GetPlant_WithValidId_ReturnsPlant()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testPlant = CreateTestPlant(5, "supplier1", "Rose", "Flowers", "Stem", "Premium", "50cm", "10", "Open", "Beautiful roses", 100, 50);
            using (var context = new DBContext(options))
            {
                context.Plants.Add(testPlant);
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlant(5);

                // Assert
                var plant = Assert.IsType<Plant>(result.Value);
                Assert.Equal(5, plant.plant_id);
                Assert.Equal("Rose", plant.productname);
                Assert.Equal("supplier1", plant.supplier_id);
                Assert.Equal(100, plant.start_price);
            }
        }

        [Fact]
        public async Task GetPlant_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.Add(CreateTestPlant(1, "supplier1"));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlant(999);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
                Assert.Null(result.Value);
            }
        }

        [Fact]
        public async Task GetPlant_WithMultiplePlants_ReturnsCorrectOne()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose"),
                    CreateTestPlant(2, "supplier2", "Tulip"),
                    CreateTestPlant(3, "supplier1", "Sunflower")
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlant(2);

                // Assert
                var plant = Assert.IsType<Plant>(result.Value);
                Assert.Equal(2, plant.plant_id);
                Assert.Equal("Tulip", plant.productname);
                Assert.Equal("supplier2", plant.supplier_id);
            }
        }

        [Fact]
        public async Task GetPlant_WithZeroId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.Add(CreateTestPlant(1, "supplier1"));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlant(0);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task GetPlant_WithNegativeId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.Add(CreateTestPlant(1, "supplier1"));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlant(-1);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        #endregion

        #region PutPlant Tests

        [Fact]
        public async Task PutPlant_WithValidIdAndPlant_UpdatesAndReturnsNoContent()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var originalPlant = CreateTestPlant(1, "supplier1", "Rose");
            using (var context = new DBContext(options))
            {
                context.Plants.Add(originalPlant);
                await context.SaveChangesAsync();
            }

            var updatedPlant = new Plant
            {
                plant_id = 1,
                supplier_id = "supplier1",
                productname = "Updated Rose",
                category = "Flowers",
                form = "Stem",
                quality = "Standard",
                min_stem = "45cm",
                stems_bunch = "8",
                maturity = "Bud",
                desc = "Updated description",
                start_price = 90,
                min_price = 45
            };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.PutPlant(1, updatedPlant);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify the update was persisted
            using (var context = new DBContext(options))
            {
                var plant = await context.Plants.FindAsync(1);
                Assert.NotNull(plant);
                Assert.Equal("Updated Rose", plant.productname);
                Assert.Equal(90, plant.start_price);
                Assert.Equal("Updated description", plant.desc);
            }
        }

        [Fact]
        public async Task PutPlant_WithMismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "supplier1",
                productname = "Rose",
                category = "Flowers",
                form = "Stem",
                quality = "Premium",
                min_stem = "50cm",
                stems_bunch = "10",
                maturity = "Open",
                start_price = 100,
                min_price = 50
            };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.PutPlant(2, plant);

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }

        [Fact]
        public async Task PutPlant_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.Add(CreateTestPlant(1, "supplier1"));
                await context.SaveChangesAsync();
            }

            var plant = new Plant
            {
                plant_id = 999,
                supplier_id = "supplier1",
                productname = "Rose",
                category = "Flowers",
                form = "Stem",
                quality = "Premium",
                min_stem = "50cm",
                stems_bunch = "10",
                maturity = "Open",
                start_price = 100,
                min_price = 50
            };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.PutPlant(999, plant);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task PutPlant_UpdatesAllProperties()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var originalPlant = CreateTestPlant(1, "supplier1", "Rose", "Flowers", "Stem", "Premium", "50cm", "10", "Open", "Original", 100, 50);
            using (var context = new DBContext(options))
            {
                context.Plants.Add(originalPlant);
                await context.SaveChangesAsync();
            }

            var updatedPlant = new Plant
            {
                plant_id = 1,
                supplier_id = "supplier2",
                productname = "Tulip",
                category = "Bulbs",
                form = "Bundle",
                quality = "Standard",
                min_stem = "40cm",
                stems_bunch = "5",
                maturity = "Bud",
                desc = "New description",
                start_price = 80,
                min_price = 40
            };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                await controller.PutPlant(1, updatedPlant);
            }

            // Verify all properties were updated
            using (var context = new DBContext(options))
            {
                var plant = await context.Plants.FindAsync(1);
                Assert.Equal("Tulip", plant.productname);
                Assert.Equal("supplier2", plant.supplier_id);
                Assert.Equal("Bulbs", plant.category);
                Assert.Equal("Bundle", plant.form);
                Assert.Equal("Standard", plant.quality);
                Assert.Equal("40cm", plant.min_stem);
                Assert.Equal("5", plant.stems_bunch);
                Assert.Equal("Bud", plant.maturity);
                Assert.Equal("New description", plant.desc);
                Assert.Equal(80, plant.start_price);
                Assert.Equal(40, plant.min_price);
            }
        }

        [Fact]
        public async Task PutPlant_WithSameData_ReturnsNoContent()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var plant = CreateTestPlant(1, "supplier1", "Rose", "Flowers", "Stem", "Premium", "50cm", "10", "Open", "Beautiful roses", 100, 50);
            using (var context = new DBContext(options))
            {
                context.Plants.Add(plant);
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act - Update with same data
                var result = await controller.PutPlant(1, plant);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }
        }

        #endregion

        #region PostPlant Tests

        [Fact]
        public async Task PostPlant_WithValidPlant_CreatesAndReturnsCreatedAtAction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var newPlant = new Plant
            {
                supplier_id = "supplier1",
                productname = "Rose",
                category = "Flowers",
                form = "Stem",
                quality = "Premium",
                min_stem = "50cm",
                stems_bunch = "10",
                maturity = "Open",
                desc = "Beautiful roses",
                start_price = 100,
                min_price = 50
            };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.PostPlant(newPlant);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal(nameof(PlantsController.GetPlant), createdAtActionResult.ActionName);
                var returnedPlant = Assert.IsType<Plant>(createdAtActionResult.Value);
                Assert.Equal("Rose", returnedPlant.productname);
                Assert.Equal("supplier1", returnedPlant.supplier_id);
                Assert.Equal(100, returnedPlant.start_price);
            }

            // Verify it was saved to the database
            using (var context = new DBContext(options))
            {
                var savedPlant = await context.Plants.FirstOrDefaultAsync(p => p.productname == "Rose");
                Assert.NotNull(savedPlant);
                Assert.Equal("supplier1", savedPlant.supplier_id);
                Assert.Equal(100, savedPlant.start_price);
            }
        }

        [Fact]
        public async Task PostPlant_AssignsIdFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var newPlant = new Plant
            {
                supplier_id = "supplier1",
                productname = "Rose",
                category = "Flowers",
                form = "Stem",
                quality = "Premium",
                min_stem = "50cm",
                stems_bunch = "10",
                maturity = "Open",
                start_price = 100,
                min_price = 50
            };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.PostPlant(newPlant);

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var plant = Assert.IsType<Plant>(createdResult.Value);
                Assert.NotEqual(0, plant.plant_id);
            }
        }

        [Fact]
        public async Task PostPlant_WithMultiplePlants_SavesAllCorrectly()
        {
            // Arrange
            var options = CreateNewContextOptions();

            var plant1 = new Plant { supplier_id = "supplier1", productname = "Rose", category = "Flowers", form = "Stem", quality = "Premium", min_stem = "50cm", stems_bunch = "10", maturity = "Open", start_price = 100, min_price = 50 };
            var plant2 = new Plant { supplier_id = "supplier1", productname = "Tulip", category = "Flowers", form = "Stem", quality = "Standard", min_stem = "40cm", stems_bunch = "5", maturity = "Bud", start_price = 80, min_price = 40 };
            var plant3 = new Plant { supplier_id = "supplier2", productname = "Sunflower", category = "Flowers", form = "Bundle", quality = "Premium", min_stem = "60cm", stems_bunch = "8", maturity = "Open", start_price = 120, min_price = 60 };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                await controller.PostPlant(plant1);
                await controller.PostPlant(plant2);
                await controller.PostPlant(plant3);
            }

            // Verify all were saved
            using (var context = new DBContext(options))
            {
                var plantsInDb = await context.Plants.ToListAsync();
                Assert.Equal(3, plantsInDb.Count);
                Assert.Single(plantsInDb.Where(p => p.productname == "Rose"));
                Assert.Single(plantsInDb.Where(p => p.productname == "Tulip"));
                Assert.Single(plantsInDb.Where(p => p.productname == "Sunflower"));
            }
        }

        [Fact]
        public async Task PostPlant_WithMinimalRequiredData_Creates()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var newPlant = new Plant
            {
                supplier_id = "supplier1",
                productname = "Rose",
                category = "Flowers",
                form = "Stem",
                quality = "Premium",
                min_stem = "50cm",
                stems_bunch = "10",
                maturity = "Open",
                start_price = 100,
                min_price = 50
            };

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.PostPlant(newPlant);

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.NotNull(createdResult.Value);
            }
        }

        #endregion

        #region DeletePlant Tests

        [Fact]
        public async Task DeletePlant_WithValidId_DeletesAndReturnsNoContent()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testPlant = CreateTestPlant(1, "supplier1", "Rose");
            using (var context = new DBContext(options))
            {
                context.Plants.Add(testPlant);
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.DeletePlant(1);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify it was deleted from database
            using (var context = new DBContext(options))
            {
                var plant = await context.Plants.FindAsync(1);
                Assert.Null(plant);
            }
        }

        [Fact]
        public async Task DeletePlant_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.Add(CreateTestPlant(1, "supplier1"));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.DeletePlant(999);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeletePlant_WithMultiplePlants_DeletesOnlySpecified()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose"),
                    CreateTestPlant(2, "supplier2", "Tulip"),
                    CreateTestPlant(3, "supplier1", "Sunflower")
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                await controller.DeletePlant(2);
            }

            // Verify only the specified one was deleted
            using (var context = new DBContext(options))
            {
                var plants = await context.Plants.ToListAsync();
                Assert.Equal(2, plants.Count);
                Assert.Null(await context.Plants.FindAsync(2));
                Assert.NotNull(await context.Plants.FindAsync(1));
                Assert.NotNull(await context.Plants.FindAsync(3));
            }
        }

        [Fact]
        public async Task DeletePlant_WithZeroId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.Add(CreateTestPlant(1, "supplier1"));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.DeletePlant(0);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeletePlant_DeletesFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose"),
                    CreateTestPlant(2, "supplier2", "Tulip")
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                await controller.DeletePlant(1);
            }

            // Verify deletion
            using (var context = new DBContext(options))
            {
                var plants = await context.Plants.ToListAsync();
                Assert.Single(plants);
                Assert.Equal(2, plants.First().plant_id);
            }
        }

        #endregion

        #region PlantExists Private Method Tests

        [Fact]
        public async Task PlantExists_WithValidId_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Plants.Add(CreateTestPlant(1, "supplier1"));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act - we test this indirectly through PutPlant
                var plant = new Plant
                {
                    plant_id = 1,
                    supplier_id = "supplier1",
                    productname = "Updated Rose",
                    category = "Flowers",
                    form = "Stem",
                    quality = "Premium",
                    min_stem = "50cm",
                    stems_bunch = "10",
                    maturity = "Open",
                    start_price = 100,
                    min_price = 50
                };
                var result = await controller.PutPlant(1, plant);

                // Assert - if PlantExists returns true, PutPlant should succeed (not return NotFound)
                Assert.IsType<NoContentResult>(result);
            }
        }

        [Fact]
        public async Task PlantExists_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var options = CreateNewContextOptions();

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act - we test this indirectly through PutPlant
                var plant = new Plant
                {
                    plant_id = 999,
                    supplier_id = "supplier1",
                    productname = "Rose",
                    category = "Flowers",
                    form = "Stem",
                    quality = "Premium",
                    min_stem = "50cm",
                    stems_bunch = "10",
                    maturity = "Open",
                    start_price = 100,
                    min_price = 50
                };
                var result = await controller.PutPlant(999, plant);

                // Assert - if PlantExists returns false, PutPlant should return NotFound
                Assert.IsType<NotFoundResult>(result);
            }
        }

        #endregion

        #region Data Integrity Tests

        [Fact]
        public async Task GetPlant_ReturnsCorrectPlantDataIntegrity()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testPlant = CreateTestPlant(1, "supplier1", "Rose", "Flowers", "Stem", "Premium", "50cm", "10", "Open", "Beautiful roses", 100, 50);
            using (var context = new DBContext(options))
            {
                context.Plants.Add(testPlant);
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.GetPlant(1);

                // Assert
                var plant = result.Value;
                Assert.Equal(1, plant.plant_id);
                Assert.Equal("supplier1", plant.supplier_id);
                Assert.Equal("Rose", plant.productname);
                Assert.Equal("Flowers", plant.category);
                Assert.Equal("Stem", plant.form);
                Assert.Equal("Premium", plant.quality);
                Assert.Equal("50cm", plant.min_stem);
                Assert.Equal("10", plant.stems_bunch);
                Assert.Equal("Open", plant.maturity);
                Assert.Equal("Beautiful roses", plant.desc);
                Assert.Equal(100, plant.start_price);
                Assert.Equal(50, plant.min_price);
            }
        }

        [Fact]
        public async Task PostPlant_PersistsAllData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var newPlant = new Plant
            {
                supplier_id = "supplier1",
                productname = "Rose",
                category = "Flowers",
                form = "Stem",
                quality = "Premium",
                min_stem = "50cm",
                stems_bunch = "10",
                maturity = "Open",
                desc = "Beautiful roses",
                start_price = 100,
                min_price = 50
            };

            int savedPlantId = 0;
            using (var context = new DBContext(options))
            {
                var controller = new PlantsController(context);

                // Act
                var result = await controller.PostPlant(newPlant);
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var plant = Assert.IsType<Plant>(createdResult.Value);
                savedPlantId = plant.plant_id;
            }

            // Assert - Verify persistence
            using (var context = new DBContext(options))
            {
                var plant = await context.Plants.FindAsync(savedPlantId);
                Assert.NotNull(plant);
                Assert.Equal("Rose", plant.productname);
                Assert.Equal("Flowers", plant.category);
                Assert.Equal(100, plant.start_price);
            }
        }

        #endregion
    }
}