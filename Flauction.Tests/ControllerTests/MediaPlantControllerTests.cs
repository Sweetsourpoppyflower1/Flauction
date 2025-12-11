using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.DTOs.Upload;
using Flauction.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Flauction.Tests.ControllerTests
{
    public class MediaPlantControllerTests
    {
        private DbContextOptions<DBContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
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

        private Mock<IWebHostEnvironment> CreateMockWebHostEnvironment(string webRoot = null)
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(m => m.WebRootPath).Returns(webRoot ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
            return mockEnv;
        }

        #region GetMedias Tests

        [Fact]
        public async Task GetMedias_ReturnsAllMediaPlants()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.MediaPlants.AddRange(
                    CreateTestMediaPlant(1, 1, "/uploads/test1.jpg", "Test 1"),
                    CreateTestMediaPlant(2, 1, "/uploads/test2.jpg", "Test 2"),
                    CreateTestMediaPlant(3, 2, "/uploads/test3.jpg", "Test 3")
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedias();

                // Assert
                var medias = Assert.IsAssignableFrom<IEnumerable<MediaPlant>>(result.Value);
                Assert.Equal(3, medias.Count());
            }
        }

        [Fact]
        public async Task GetMedias_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedias();

                // Assert
                var medias = Assert.IsAssignableFrom<IEnumerable<MediaPlant>>(result.Value);
                Assert.Empty(medias);
            }
        }

        [Fact]
        public async Task GetMedias_WithMultipleMediaPlants_ReturnsAllWithCorrectData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.MediaPlants.AddRange(
                    CreateTestMediaPlant(1, 1, "/uploads/image1.png", "Image 1", false),
                    CreateTestMediaPlant(2, 2, "/uploads/image2.gif", "Image 2", true),
                    CreateTestMediaPlant(3, 3, "/uploads/image3.jpg", "Image 3", false)
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedias();

                // Assert
                var medias = result.Value.ToList();
                Assert.Equal(3, medias.Count);
                Assert.Contains(medias, m => m.url == "/uploads/image1.png" && m.plant_id == 1);
                Assert.Contains(medias, m => m.url == "/uploads/image2.gif" && m.is_primary);
                Assert.Contains(medias, m => m.url == "/uploads/image3.jpg" && m.plant_id == 3);
            }
        }

        #endregion

        #region GetMedia Tests

        [Fact]
        public async Task GetMedia_WithValidId_ReturnsMediaPlant()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testMedia = CreateTestMediaPlant(5, 2, "/uploads/specific.jpg", "Specific Media");
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(testMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedia(5);

                // Assert
                var media = Assert.IsType<MediaPlant>(result.Value);
                Assert.Equal(5, media.mediaplant_id);
                Assert.Equal(2, media.plant_id);
                Assert.Equal("/uploads/specific.jpg", media.url);
                Assert.Equal("Specific Media", media.alt_text);
            }
        }

        [Fact]
        public async Task GetMedia_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(CreateTestMediaPlant(1, 1));
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedia(999);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
                Assert.Null(result.Value);
            }
        }

        [Fact]
        public async Task GetMedia_WithMultipleMediaPlants_ReturnsCorrectOne()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.MediaPlants.AddRange(
                    CreateTestMediaPlant(1, 1, "/uploads/test1.jpg", "Test 1"),
                    CreateTestMediaPlant(2, 2, "/uploads/test2.jpg", "Test 2"),
                    CreateTestMediaPlant(3, 1, "/uploads/test3.jpg", "Test 3")
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedia(2);

                // Assert
                var media = Assert.IsType<MediaPlant>(result.Value);
                Assert.Equal(2, media.mediaplant_id);
                Assert.Equal(2, media.plant_id);
                Assert.Equal("/uploads/test2.jpg", media.url);
            }
        }

        #endregion

        #region PutMedia Tests

        [Fact]
        public async Task PutMedia_WithValidIdAndMedia_UpdatesAndReturnsNoContent()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var originalMedia = CreateTestMediaPlant(1, 1, "/uploads/original.jpg", "Original");
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(originalMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();
            var updatedMedia = new MediaPlant
            {
                mediaplant_id = 1,
                plant_id = 1,
                url = "/uploads/updated.jpg",
                alt_text = "Updated",
                is_primary = false
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.PutMedia(1, updatedMedia);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify the update was persisted
            using (var context = new DBContext(options))
            {
                var media = await context.MediaPlants.FindAsync(1);
                Assert.NotNull(media);
                Assert.Equal("/uploads/updated.jpg", media.url);
                Assert.Equal("Updated", media.alt_text);
            }
        }

        [Fact]
        public async Task PutMedia_WithMismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();
            var media = new MediaPlant
            {
                mediaplant_id = 1,
                plant_id = 1,
                url = "/uploads/test.jpg",
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.PutMedia(2, media);

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }

        [Fact]
        public async Task PutMedia_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(CreateTestMediaPlant(1, 1));
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();
            var media = new MediaPlant
            {
                mediaplant_id = 999,
                plant_id = 1,
                url = "/uploads/test.jpg",
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.PutMedia(999, media);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task PutMedia_UpdatesAllProperties()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var originalMedia = CreateTestMediaPlant(1, 1, "/uploads/original.jpg", "Original Alt", false);
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(originalMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();
            var updatedMedia = new MediaPlant
            {
                mediaplant_id = 1,
                plant_id = 1,
                url = "/uploads/new-url.png",
                alt_text = "New Alt Text",
                is_primary = true
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                await controller.PutMedia(1, updatedMedia);
            }

            // Verify all properties were updated
            using (var context = new DBContext(options))
            {
                var media = await context.MediaPlants.FindAsync(1);
                Assert.Equal("/uploads/new-url.png", media.url);
                Assert.Equal("New Alt Text", media.alt_text);
                Assert.True(media.is_primary);
            }
        }

        #endregion

        #region PostMedia Tests

        [Fact]
        public async Task PostMedia_WithValidMedia_CreatesAndReturnsCreatedAtAction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();
            var newMedia = new MediaPlant
            {
                plant_id = 1,
                url = "/uploads/new.jpg",
                alt_text = "New Media"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.PostMedia(newMedia);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal(nameof(MediaPlantController.GetMedia), createdAtActionResult.ActionName);
                var returnedMedia = Assert.IsType<MediaPlant>(createdAtActionResult.Value);
                Assert.Equal("/uploads/new.jpg", returnedMedia.url);
                Assert.Equal("New Media", returnedMedia.alt_text);
                Assert.Equal(1, returnedMedia.plant_id);
            }

            // Verify it was saved to the database
            using (var context = new DBContext(options))
            {
                var savedMedia = await context.MediaPlants.FirstOrDefaultAsync(m => m.url == "/uploads/new.jpg");
                Assert.NotNull(savedMedia);
                Assert.Equal("New Media", savedMedia.alt_text);
                Assert.Equal(1, savedMedia.plant_id);
            }
        }

        [Fact]
        public async Task PostMedia_AssignsIdFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();
            var newMedia = new MediaPlant
            {
                plant_id = 1,
                url = "/uploads/test.jpg",
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.PostMedia(newMedia);

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var media = Assert.IsType<MediaPlant>(createdResult.Value);
                Assert.NotEqual(0, media.mediaplant_id);
            }
        }

        [Fact]
        public async Task PostMedia_WithMultipleMediaPlants_SavesAllCorrectly()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();

            var media1 = new MediaPlant { plant_id = 1, url = "/uploads/media1.jpg", alt_text = "Media 1" };
            var media2 = new MediaPlant { plant_id = 1, url = "/uploads/media2.jpg", alt_text = "Media 2" };
            var media3 = new MediaPlant { plant_id = 2, url = "/uploads/media3.jpg", alt_text = "Media 3" };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                await controller.PostMedia(media1);
                await controller.PostMedia(media2);
                await controller.PostMedia(media3);
            }

            // Verify all were saved
            using (var context = new DBContext(options))
            {
                var mediasInDb = await context.MediaPlants.ToListAsync();
                Assert.Equal(3, mediasInDb.Count);
                Assert.Single(mediasInDb.Where(m => m.url == "/uploads/media1.jpg"));
                Assert.Single(mediasInDb.Where(m => m.url == "/uploads/media2.jpg"));
                Assert.Single(mediasInDb.Where(m => m.url == "/uploads/media3.jpg"));
            }
        }

        #endregion

        #region Upload Tests

        [Fact]
        public async Task Upload_WithValidFile_CreatesMediaPlantAndReturnsCreatedAtAction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test file content";
                var fileName = "test.jpg";
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };

                var dto = new MediaPlantUploadDTO
                {
                    file = formFile,
                    plant_id = 1,
                    alt_text = "Test Image"
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    Assert.Equal(nameof(MediaPlantController.GetMedia), createdAtActionResult.ActionName);
                    var returnedMedia = Assert.IsType<MediaPlant>(createdAtActionResult.Value);
                    Assert.Equal("Test Image", returnedMedia.alt_text);
                    Assert.Equal(1, returnedMedia.plant_id);
                    Assert.StartsWith("/uploads/", returnedMedia.url);
                }

                // Verify file was created and saved to database
                using (var context = new DBContext(options))
                {
                    var savedMedia = await context.MediaPlants.FirstOrDefaultAsync(m => m.alt_text == "Test Image");
                    Assert.NotNull(savedMedia);
                    Assert.StartsWith("/uploads/", savedMedia.url);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task Upload_WithNullFile_ReturnsBadRequest()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();
            var dto = new MediaPlantUploadDTO
            {
                file = null,
                plant_id = 1,
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.Upload(dto);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
                Assert.Equal("No file uploaded.", badRequestResult.Value);
            }
        }

        [Fact]
        public async Task Upload_WithEmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();

            var stream = new MemoryStream();
            var formFile = new FormFile(stream, 0, 0, "file", "empty.jpg");

            var dto = new MediaPlantUploadDTO
            {
                file = formFile,
                plant_id = 1,
                alt_text = "Empty File"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.Upload(dto);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
                Assert.Equal("No file uploaded.", badRequestResult.Value);
            }
        }

        [Fact]
        public async Task Upload_UsesFileNameExtension()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test";
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile = new FormFile(stream, 0, stream.Length, "file", "image.png");

                var dto = new MediaPlantUploadDTO
                {
                    file = formFile,
                    plant_id = 1,
                    alt_text = "PNG Image"
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    var media = Assert.IsType<MediaPlant>(createdResult.Value);
                    Assert.EndsWith(".png", media.url);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task Upload_WithoutAltText_UsesFileName()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test";
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile = new FormFile(stream, 0, stream.Length, "file", "my_image.jpg");

                var dto = new MediaPlantUploadDTO
                {
                    file = formFile,
                    plant_id = 1,
                    alt_text = null
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    var media = Assert.IsType<MediaPlant>(createdResult.Value);
                    Assert.Equal("my_image", media.alt_text);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task Upload_WithWhitespaceAltText_UsesFileName()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test";
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile = new FormFile(stream, 0, stream.Length, "file", "photo.jpg");

                var dto = new MediaPlantUploadDTO
                {
                    file = formFile,
                    plant_id = 1,
                    alt_text = "   "
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    var media = Assert.IsType<MediaPlant>(createdResult.Value);
                    Assert.Equal("photo", media.alt_text);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task Upload_WithPrimaryFlagSet_UnsetsPreviousPrimary()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Create existing primary image
                using (var context = new DBContext(options))
                {
                    context.MediaPlants.Add(CreateTestMediaPlant(1, 1, "/uploads/primary.jpg", "Primary Image", true));
                    await context.SaveChangesAsync();
                }

                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test";
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile = new FormFile(stream, 0, stream.Length, "file", "new_primary.jpg");

                var dto = new MediaPlantUploadDTO
                {
                    file = formFile,
                    plant_id = 1,
                    alt_text = "New Primary",
                    is_primary = true
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    await controller.Upload(dto);
                }

                // Verify the previous primary was unset
                using (var context = new DBContext(options))
                {
                    var oldPrimary = await context.MediaPlants.FindAsync(1);
                    Assert.False(oldPrimary.is_primary);

                    var newPrimary = await context.MediaPlants.FirstOrDefaultAsync(m => m.alt_text == "New Primary");
                    Assert.NotNull(newPrimary);
                    Assert.True(newPrimary.is_primary);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task Upload_WithMultiplePrimariesForSamePlant_UnsetsOnlyOldPrimary()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Create multiple images, some primary
                using (var context = new DBContext(options))
                {
                    context.MediaPlants.AddRange(
                        CreateTestMediaPlant(1, 1, "/uploads/primary1.jpg", "Primary 1", true),
                        CreateTestMediaPlant(2, 1, "/uploads/secondary.jpg", "Secondary", false),
                        CreateTestMediaPlant(3, 2, "/uploads/other.jpg", "Other Plant", true)
                    );
                    await context.SaveChangesAsync();
                }

                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test";
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile = new FormFile(stream, 0, stream.Length, "file", "primary2.jpg");

                var dto = new MediaPlantUploadDTO
                {
                    file = formFile,
                    plant_id = 1,
                    alt_text = "Primary 2",
                    is_primary = true
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    await controller.Upload(dto);
                }

                // Verify only the old primary for plant 1 was unset, not plant 2
                using (var context = new DBContext(options))
                {
                    var oldPrimary = await context.MediaPlants.FindAsync(1);
                    Assert.False(oldPrimary.is_primary);

                    var otherPlantPrimary = await context.MediaPlants.FindAsync(3);
                    Assert.True(otherPlantPrimary.is_primary);

                    var newPrimary = await context.MediaPlants.FirstOrDefaultAsync(m => m.alt_text == "Primary 2");
                    Assert.NotNull(newPrimary);
                    Assert.True(newPrimary.is_primary);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task Upload_WithNonPrimaryFlag_KeepsOtherPrimary()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Create existing primary image
                using (var context = new DBContext(options))
                {
                    context.MediaPlants.Add(CreateTestMediaPlant(1, 1, "/uploads/primary.jpg", "Primary Image", true));
                    await context.SaveChangesAsync();
                }

                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test";
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile = new FormFile(stream, 0, stream.Length, "file", "secondary.jpg");

                var dto = new MediaPlantUploadDTO
                {
                    file = formFile,
                    plant_id = 1,
                    alt_text = "Secondary",
                    is_primary = false
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    await controller.Upload(dto);
                }

                // Verify the primary was not changed
                using (var context = new DBContext(options))
                {
                    var primary = await context.MediaPlants.FirstOrDefaultAsync(m => m.is_primary && m.plant_id == 1);
                    Assert.NotNull(primary);
                    Assert.Equal("Primary Image", primary.alt_text);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task Upload_CreatesUniqueFilenames()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                var fileContent = "test";
                var stream1 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile1 = new FormFile(stream1, 0, stream1.Length, "file", "duplicate.jpg");

                var stream2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
                var formFile2 = new FormFile(stream2, 0, stream2.Length, "file", "duplicate.jpg");

                var dto1 = new MediaPlantUploadDTO
                {
                    file = formFile1,
                    plant_id = 1,
                    alt_text = "First Upload"
                };

                var dto2 = new MediaPlantUploadDTO
                {
                    file = formFile2,
                    plant_id = 1,
                    alt_text = "Second Upload"
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    var result1 = await controller.Upload(dto1);
                    var result2 = await controller.Upload(dto2);

                    // Assert
                    var media1 = Assert.IsType<MediaPlant>(
                        Assert.IsType<CreatedAtActionResult>(result1.Result).Value);
                    var media2 = Assert.IsType<MediaPlant>(
                        Assert.IsType<CreatedAtActionResult>(result2.Result).Value);

                    Assert.NotEqual(media1.url, media2.url);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        #endregion

        #region DeleteMedia Tests

        [Fact]
        public async Task DeleteMedia_WithValidId_DeletesAndReturnsNoContent()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testMedia = CreateTestMediaPlant(1, 1, "/uploads/test.jpg", "Test");
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(testMedia);
                await context.SaveChangesAsync();
            }

            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    var result = await controller.DeleteMedia(1);

                    // Assert
                    Assert.IsType<NoContentResult>(result);
                }

                // Verify it was deleted from database
                using (var context = new DBContext(options))
                {
                    var media = await context.MediaPlants.FindAsync(1);
                    Assert.Null(media);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task DeleteMedia_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(CreateTestMediaPlant(1, 1));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.DeleteMedia(999);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteMedia_WithMultipleMediaPlants_DeletesOnlySpecified()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.MediaPlants.AddRange(
                    CreateTestMediaPlant(1, 1, "/uploads/test1.jpg", "Test 1"),
                    CreateTestMediaPlant(2, 1, "/uploads/test2.jpg", "Test 2"),
                    CreateTestMediaPlant(3, 2, "/uploads/test3.jpg", "Test 3")
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                await controller.DeleteMedia(2);
            }

            // Verify only the specified one was deleted
            using (var context = new DBContext(options))
            {
                var medias = await context.MediaPlants.ToListAsync();
                Assert.Equal(2, medias.Count);
                Assert.Null(await context.MediaPlants.FindAsync(2));
                Assert.NotNull(await context.MediaPlants.FindAsync(1));
                Assert.NotNull(await context.MediaPlants.FindAsync(3));
            }
        }

        [Fact]
        public async Task DeleteMedia_WhenFileDoesNotExist_StillDeletesFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testMedia = CreateTestMediaPlant(1, 1, "/uploads/nonexistent.jpg", "Test");
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(testMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act
                var result = await controller.DeleteMedia(1);

                // Assert - should not throw, should delete from database
                Assert.IsType<NoContentResult>(result);
            }

            // Verify it was deleted from database despite file not existing
            using (var context = new DBContext(options))
            {
                var media = await context.MediaPlants.FindAsync(1);
                Assert.Null(media);
            }
        }

        [Fact]
        public async Task DeleteMedia_WithExistingFile_DeletesBothFileAndRecord()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var uploadsDir = Path.Combine(tempDir, "uploads");
            Directory.CreateDirectory(uploadsDir);

            try
            {
                // Create a test file
                var testFileName = Guid.NewGuid().ToString() + ".jpg";
                var filePath = Path.Combine(uploadsDir, testFileName);
                await System.IO.File.WriteAllTextAsync(filePath, "test content");

                var testMedia = CreateTestMediaPlant(1, 1, $"/uploads/{testFileName}", "Test");
                using (var context = new DBContext(options))
                {
                    context.MediaPlants.Add(testMedia);
                    await context.SaveChangesAsync();
                }

                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                using (var context = new DBContext(options))
                {
                    var controller = new MediaPlantController(context, mockEnv.Object);

                    // Act
                    var result = await controller.DeleteMedia(1);

                    // Assert
                    Assert.IsType<NoContentResult>(result);
                }

                // Verify file was deleted
                Assert.False(System.IO.File.Exists(filePath));

                // Verify record was deleted
                using (var context = new DBContext(options))
                {
                    var media = await context.MediaPlants.FindAsync(1);
                    Assert.Null(media);
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        #endregion

        #region MediaExists Private Method Tests

        [Fact]
        public async Task MediaExists_WithValidId_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.MediaPlants.Add(CreateTestMediaPlant(1, 1));
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act - we test this indirectly through PutMedia
                var media = new MediaPlant
                {
                    mediaplant_id = 1,
                    plant_id = 1,
                    url = "/uploads/updated.jpg",
                    alt_text = "Updated"
                };
                var result = await controller.PutMedia(1, media);

                // Assert - if MediaExists returns true, PutMedia should succeed (not return NotFound)
                Assert.IsType<NoContentResult>(result);
            }
        }

        [Fact]
        public async Task MediaExists_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaPlantController(context, mockEnv.Object);

                // Act - we test this indirectly through PutMedia
                var media = new MediaPlant
                {
                    mediaplant_id = 999,
                    plant_id = 1,
                    url = "/uploads/test.jpg",
                    alt_text = "Test"
                };
                var result = await controller.PutMedia(999, media);

                // Assert - if MediaExists returns false, PutMedia should return NotFound
                Assert.IsType<NotFoundResult>(result);
            }
        }

        #endregion
    }
}