using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Controllers;
using Flauction.Data;
using Flauction.DTOs.Output;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Flauction.Tests.ControllerTests
{
    public class PlantOverviewControllerTests
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

        private Supplier CreateTestSupplier(string id = "supplier1", string name = "Test Supplier")
        {
            return new Supplier
            {
                Id = id,
                name = name,
                address = "123 Main St",
                postalcode = "12345",
                country = "Netherlands",
                iban = "NL91ABNA0417164300",
                UserName = id,
                Email = $"{id}@test.com"
            };
        }

        private MediaPlant CreateTestMediaPlant(int id = 1, int plantId = 1, string url = "/uploads/test.jpg",
            string altText = "Test Image", bool isPrimary = false)
        {
            return new MediaPlant
            {
                mediaplant_id = id,
                plant_id = plantId,
                url = url,
                alt_text = altText,
                is_primary = isPrimary
            };
        }

        #region GetOverview Tests

        [Fact]
        public async Task GetOverview_ReturnsAllPlantsAsOverviewDTO()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose"),
                    CreateTestPlant(2, "supplier1", "Tulip"),
                    CreateTestPlant(3, "supplier1", "Sunflower")
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<IEnumerable<PlantOverviewDTO>>(okResult.Value);
                Assert.Equal(3, overviews.Count());
            }
        }

        [Fact]
        public async Task GetOverview_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var options = CreateNewContextOptions();

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<IEnumerable<PlantOverviewDTO>>(okResult.Value);
                Assert.Empty(overviews);
            }
        }

        [Fact]
        public async Task GetOverview_IncludesCorrectPlantData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Premium Flowers");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose", "Flowers", "Stem", "Premium", 
                    "50cm", "10", "Open", "Beautiful red roses", 100, 50);
                context.Plants.Add(plant);
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Equal(1, overview.PlantId);
                Assert.Equal("Rose", overview.PlantName);
                Assert.Equal("Rose", overview.ProductName);
                Assert.Equal("Premium Flowers", overview.Supplier);
                Assert.Equal("Flowers", overview.Category);
                Assert.Equal("Stem", overview.Form);
                Assert.Equal("Premium", overview.Quality);
                Assert.Equal("50cm", overview.MinStem);
                Assert.Equal("10", overview.StemsBunch);
                Assert.Equal("Open", overview.Maturity);
                Assert.Equal("Beautiful red roses", overview.Desc);
                Assert.Equal(50, overview.MinPrice);
                Assert.Equal(100, overview.MaxPrice);
            }
        }

        [Fact]
        public async Task GetOverview_WithMultiplePlants_ReturnsAllWithCorrectData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier1 = CreateTestSupplier("supplier1", "Supplier One");
                var supplier2 = CreateTestSupplier("supplier2", "Supplier Two");
                context.Suppliers.AddRange(supplier1, supplier2);

                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose", "Flowers", "Stem", "Premium", "50cm", "10", "Open", "Beautiful roses", 100, 50),
                    CreateTestPlant(2, "supplier2", "Tulip", "Flowers", "Stem", "Standard", "40cm", "5", "Bud", "Lovely tulips", 80, 40),
                    CreateTestPlant(3, "supplier1", "Sunflower", "Flowers", "Bundle", "Premium", "60cm", "8", "Open", "Bright sunflowers", 120, 60)
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);

                Assert.Equal(3, overviews.Count);
                Assert.Contains(overviews, p => p.PlantName == "Rose" && p.Supplier == "Supplier One");
                Assert.Contains(overviews, p => p.PlantName == "Tulip" && p.MaxPrice == 80);
                Assert.Contains(overviews, p => p.PlantName == "Sunflower" && p.PlantId == 3);
            }
        }

        [Fact]
        public async Task GetOverview_IncludesPrimaryImageUrl()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose");
                context.Plants.Add(plant);

                var primaryMedia = CreateTestMediaPlant(1, 1, "/uploads/rose-primary.jpg", "Rose", true);
                var secondaryMedia = CreateTestMediaPlant(2, 1, "/uploads/rose-secondary.jpg", "Rose", false);
                context.MediaPlants.AddRange(primaryMedia, secondaryMedia);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Equal("/uploads/rose-primary.jpg", overview.ImageUrl);
            }
        }

        [Fact]
        public async Task GetOverview_WithoutPrimaryImage_ReturnsNullImageUrl()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose");
                context.Plants.Add(plant);

                var secondaryMedia = CreateTestMediaPlant(1, 1, "/uploads/rose-secondary.jpg", "Rose", false);
                context.MediaPlants.Add(secondaryMedia);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Null(overview.ImageUrl);
            }
        }

        [Fact]
        public async Task GetOverview_WithoutAnyImages_ReturnsNullImageUrl()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose");
                context.Plants.Add(plant);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Null(overview.ImageUrl);
            }
        }

        [Fact]
        public async Task GetOverview_IgnoresSecondaryImages()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose");
                context.Plants.Add(plant);

                var secondaryMedia1 = CreateTestMediaPlant(1, 1, "/uploads/rose-1.jpg", "Rose", false);
                var secondaryMedia2 = CreateTestMediaPlant(2, 1, "/uploads/rose-2.jpg", "Rose", false);
                context.MediaPlants.AddRange(secondaryMedia1, secondaryMedia2);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Null(overview.ImageUrl);
            }
        }

        [Fact]
        public async Task GetOverview_WithMultiplePrimaryImages_ReturnsFirstOne()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose");
                context.Plants.Add(plant);

                // Add multiple primary images (edge case)
                var primaryMedia1 = CreateTestMediaPlant(1, 1, "/uploads/rose-first.jpg", "Rose", true);
                var primaryMedia2 = CreateTestMediaPlant(2, 1, "/uploads/rose-second.jpg", "Rose", true);
                context.MediaPlants.AddRange(primaryMedia1, primaryMedia2);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                // Should return one of the primary images (FirstOrDefault behavior)
                Assert.NotNull(overview.ImageUrl);
                Assert.StartsWith("/uploads/rose-", overview.ImageUrl);
            }
        }

        [Fact]
        public async Task GetOverview_ReturnsOkResult()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose");
                context.Plants.Add(plant);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Fact]
        public async Task GetOverview_MapsSupplierId_ToSupplierName()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier1 = CreateTestSupplier("supplier1", "Flower Shop A");
                var supplier2 = CreateTestSupplier("supplier2", "Flower Shop B");
                context.Suppliers.AddRange(supplier1, supplier2);

                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose"),
                    CreateTestPlant(2, "supplier2", "Tulip")
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);

                var roseOverview = overviews.First(p => p.PlantName == "Rose");
                var tulipOverview = overviews.First(p => p.PlantName == "Tulip");

                Assert.Equal("Flower Shop A", roseOverview.Supplier);
                Assert.Equal("Flower Shop B", tulipOverview.Supplier);
            }
        }

        [Fact]
        public async Task GetOverview_WithNonexistentSupplier_ReturnsNullSupplierName()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                // Add plant with supplier_id that doesn't exist in Suppliers table
                var plant = CreateTestPlant(1, "nonexistent_supplier", "Rose");
                context.Plants.Add(plant);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Null(overview.Supplier);
            }
        }

        [Fact]
        public async Task GetOverview_PreservesPlantPriceMapping()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                context.Plants.AddRange(
                    CreateTestPlant(1, "supplier1", "Rose", startPrice: 150, minPrice: 75),
                    CreateTestPlant(2, "supplier1", "Tulip", startPrice: 200, minPrice: 100),
                    CreateTestPlant(3, "supplier1", "Sunflower", startPrice: 120, minPrice: 60)
                );
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);

                var roseOverview = overviews.First(p => p.PlantName == "Rose");
                var tulipOverview = overviews.First(p => p.PlantName == "Tulip");
                var sunflowerOverview = overviews.First(p => p.PlantName == "Sunflower");

                Assert.Equal(75, roseOverview.MinPrice);
                Assert.Equal(150, roseOverview.MaxPrice);
                Assert.Equal(100, tulipOverview.MinPrice);
                Assert.Equal(200, tulipOverview.MaxPrice);
                Assert.Equal(60, sunflowerOverview.MinPrice);
                Assert.Equal(120, sunflowerOverview.MaxPrice);
            }
        }

        [Fact]
        public async Task GetOverview_PreservesAllPlantAttributes()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                var plant = CreateTestPlant(1, "supplier1", "Rose", "Exotic Flowers", "Premium Bundle", 
                    "AAA Grade", "80cm", "20", "Fully Open", "Luxury red roses", 250, 150);
                context.Plants.Add(plant);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Equal("Rose", overview.PlantName);
                Assert.Equal("Exotic Flowers", overview.Category);
                Assert.Equal("Premium Bundle", overview.Form);
                Assert.Equal("AAA Grade", overview.Quality);
                Assert.Equal("80cm", overview.MinStem);
                Assert.Equal("20", overview.StemsBunch);
                Assert.Equal("Fully Open", overview.Maturity);
                Assert.Equal("Luxury red roses", overview.Desc);
                Assert.Equal(150, overview.MinPrice);
                Assert.Equal(250, overview.MaxPrice);
            }
        }

        [Fact]
        public async Task GetOverview_WithNullDescriptionInPlant_ReturnsNullDesc()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

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
                    desc = null,
                    start_price = 100,
                    min_price = 50
                };
                context.Plants.Add(plant);

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);
                var overview = overviews.First();

                Assert.Null(overview.Desc);
            }
        }

        #endregion

        #region Complex Scenario Tests

        [Fact]
        public async Task GetOverview_WithLargeDatabaseOfPlants_ReturnsAllCorrectly()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                // Add 100 plants
                for (int i = 1; i <= 100; i++)
                {
                    context.Plants.Add(CreateTestPlant(i, "supplier1", $"Plant{i}"));
                }

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);

                Assert.Equal(100, overviews.Count);
                for (int i = 1; i <= 100; i++)
                {
                    Assert.Single(overviews.Where(p => p.PlantId == i));
                }
            }
        }

        [Fact]
        public async Task GetOverview_WithMixedImageStates_ReturnsCorrectImageUrls()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                // Plant 1: Has primary image
                context.Plants.Add(CreateTestPlant(1, "supplier1", "Rose"));
                context.MediaPlants.Add(CreateTestMediaPlant(1, 1, "/uploads/rose-primary.jpg", "Rose", true));

                // Plant 2: Has only secondary images
                context.Plants.Add(CreateTestPlant(2, "supplier1", "Tulip"));
                context.MediaPlants.Add(CreateTestMediaPlant(2, 2, "/uploads/tulip-secondary.jpg", "Tulip", false));

                // Plant 3: Has no images
                context.Plants.Add(CreateTestPlant(3, "supplier1", "Sunflower"));

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);

                var roseOverview = overviews.First(p => p.PlantName == "Rose");
                var tulipOverview = overviews.First(p => p.PlantName == "Tulip");
                var sunflowerOverview = overviews.First(p => p.PlantName == "Sunflower");

                Assert.NotNull(roseOverview.ImageUrl);
                Assert.Null(tulipOverview.ImageUrl);
                Assert.Null(sunflowerOverview.ImageUrl);
            }
        }

        [Fact]
        public async Task GetOverview_CorrectlyOrdersAndIndexesAllPlants()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                var supplier = CreateTestSupplier("supplier1", "Test Supplier");
                context.Suppliers.Add(supplier);

                context.Plants.AddRange(
                    CreateTestPlant(10, "supplier1", "Plant10"),
                    CreateTestPlant(5, "supplier1", "Plant5"),
                    CreateTestPlant(15, "supplier1", "Plant15")
                );

                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new PlantOverviewController(context);

                // Act
                var result = await controller.GetOverview();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var overviews = Assert.IsAssignableFrom<List<PlantOverviewDTO>>(okResult.Value);

                Assert.Equal(3, overviews.Count);
                Assert.Contains(overviews, p => p.PlantId == 10);
                Assert.Contains(overviews, p => p.PlantId == 5);
                Assert.Contains(overviews, p => p.PlantId == 15);
            }
        }

        #endregion
    }
}