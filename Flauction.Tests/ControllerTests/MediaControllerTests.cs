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
    public class MediaControllerTests
    {
        private DbContextOptions<DBContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private Media CreateTestMedia(int id = 1, string url = "/uploads/test.jpg", string altText = "Test Image")
        {
            return new Media
            {
                media_id = id,
                url = url,
                alt_text = altText
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
        public async Task GetMedias_ReturnsAllMedias()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Medias.AddRange(
                    CreateTestMedia(1, "/uploads/test1.jpg", "Test 1"),
                    CreateTestMedia(2, "/uploads/test2.jpg", "Test 2"),
                    CreateTestMedia(3, "/uploads/test3.jpg", "Test 3")
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedias();

                // Assert
                var medias = Assert.IsAssignableFrom<IEnumerable<Media>>(result.Value);
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
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedias();

                // Assert
                var medias = Assert.IsAssignableFrom<IEnumerable<Media>>(result.Value);
                Assert.Empty(medias);
            }
        }

        [Fact]
        public async Task GetMedias_WithMultipleMedias_ReturnsAllWithCorrectData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Medias.AddRange(
                    CreateTestMedia(1, "/uploads/image1.png", "Image 1"),
                    CreateTestMedia(2, "/uploads/image2.gif", "Image 2")
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedias();

                // Assert
                var medias = result.Value.ToList();
                Assert.Equal(2, medias.Count);
                Assert.Contains(medias, m => m.url == "/uploads/image1.png");
                Assert.Contains(medias, m => m.url == "/uploads/image2.gif");
            }
        }

        #endregion

        #region GetMedia Tests

        [Fact]
        public async Task GetMedia_WithValidId_ReturnsMedia()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testMedia = CreateTestMedia(5, "/uploads/specific.jpg", "Specific Media");
            using (var context = new DBContext(options))
            {
                context.Medias.Add(testMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedia(5);

                // Assert
                var media = Assert.IsType<Media>(result.Value);
                Assert.Equal(5, media.media_id);
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
                context.Medias.Add(CreateTestMedia(1, "/uploads/test.jpg", "Test"));
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedia(999);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
                Assert.Null(result.Value);
            }
        }

        [Fact]
        public async Task GetMedia_WithMultipleMedias_ReturnsCorrectOne()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Medias.AddRange(
                    CreateTestMedia(1, "/uploads/test1.jpg", "Test 1"),
                    CreateTestMedia(2, "/uploads/test2.jpg", "Test 2"),
                    CreateTestMedia(3, "/uploads/test3.jpg", "Test 3")
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.GetMedia(2);

                // Assert
                var media = Assert.IsType<Media>(result.Value);
                Assert.Equal(2, media.media_id);
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
            var originalMedia = CreateTestMedia(1, "/uploads/original.jpg", "Original");
            using (var context = new DBContext(options))
            {
                context.Medias.Add(originalMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();
            var updatedMedia = new Media
            {
                media_id = 1,
                url = "/uploads/updated.jpg",
                alt_text = "Updated"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.PutMedia(1, updatedMedia);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }

            // Verify the update was persisted
            using (var context = new DBContext(options))
            {
                var media = await context.Medias.FindAsync(1);
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
            var media = new Media
            {
                media_id = 1,
                url = "/uploads/test.jpg",
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

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
                context.Medias.Add(CreateTestMedia(1, "/uploads/test.jpg", "Test"));
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();
            var media = new Media
            {
                media_id = 999,
                url = "/uploads/test.jpg",
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

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
            var originalMedia = CreateTestMedia(1, "/uploads/original.jpg", "Original Alt");
            using (var context = new DBContext(options))
            {
                context.Medias.Add(originalMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();
            var updatedMedia = new Media
            {
                media_id = 1,
                url = "/uploads/new-url.png",
                alt_text = "New Alt Text"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                await controller.PutMedia(1, updatedMedia);
            }

            // Verify both properties were updated
            using (var context = new DBContext(options))
            {
                var media = await context.Medias.FindAsync(1);
                Assert.Equal("/uploads/new-url.png", media.url);
                Assert.Equal("New Alt Text", media.alt_text);
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
            var newMedia = new Media
            {
                url = "/uploads/new.jpg",
                alt_text = "New Media"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.PostMedia(newMedia);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal(nameof(MediaController.GetMedia), createdAtActionResult.ActionName);
                var returnedMedia = Assert.IsType<Media>(createdAtActionResult.Value);
                Assert.Equal("/uploads/new.jpg", returnedMedia.url);
                Assert.Equal("New Media", returnedMedia.alt_text);
            }

            // Verify it was saved to the database
            using (var context = new DBContext(options))
            {
                var savedMedia = await context.Medias.FirstOrDefaultAsync(m => m.url == "/uploads/new.jpg");
                Assert.NotNull(savedMedia);
                Assert.Equal("New Media", savedMedia.alt_text);
            }
        }

        [Fact]
        public async Task PostMedia_AssignsIdFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();
            var newMedia = new Media
            {
                url = "/uploads/test.jpg",
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.PostMedia(newMedia);

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var media = Assert.IsType<Media>(createdResult.Value);
                Assert.NotEqual(0, media.media_id);
            }
        }

        [Fact]
        public async Task PostMedia_WithMultipleMedias_SavesAllCorrectly()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockEnv = CreateMockWebHostEnvironment();

            var media1 = new Media { url = "/uploads/media1.jpg", alt_text = "Media 1" };
            var media2 = new Media { url = "/uploads/media2.jpg", alt_text = "Media 2" };
            var media3 = new Media { url = "/uploads/media3.jpg", alt_text = "Media 3" };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                await controller.PostMedia(media1);
                await controller.PostMedia(media2);
                await controller.PostMedia(media3);
            }

            // Verify all were saved
            using (var context = new DBContext(options))
            {
                var mediasInDb = await context.Medias.ToListAsync();
                Assert.Equal(3, mediasInDb.Count);
                Assert.Single(mediasInDb.Where(m => m.url == "/uploads/media1.jpg"));
                Assert.Single(mediasInDb.Where(m => m.url == "/uploads/media2.jpg"));
                Assert.Single(mediasInDb.Where(m => m.url == "/uploads/media3.jpg"));
            }
        }

        #endregion

        #region Upload Tests

        [Fact]
        public async Task Upload_WithValidFile_CreatesMediaAndReturnsCreatedAtAction()
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

                var dto = new MediaUpload
                {
                    file = formFile,
                    alt_text = "Test Image"
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    Assert.Equal(nameof(MediaController.GetMedia), createdAtActionResult.ActionName);
                    var returnedMedia = Assert.IsType<Media>(createdAtActionResult.Value);
                    Assert.Equal("Test Image", returnedMedia.alt_text);
                    Assert.StartsWith("/uploads/", returnedMedia.url);
                }

                // Verify file was created and saved to database
                using (var context = new DBContext(options))
                {
                    var savedMedia = await context.Medias.FirstOrDefaultAsync(m => m.alt_text == "Test Image");
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
            var dto = new MediaUpload
            {
                file = null,
                alt_text = "Test"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

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

            var dto = new MediaUpload
            {
                file = formFile,
                alt_text = "Empty File"
            };

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

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

                var dto = new MediaUpload
                {
                    file = formFile,
                    alt_text = "PNG Image"
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    var media = Assert.IsType<Media>(createdResult.Value);
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

                var dto = new MediaUpload
                {
                    file = formFile,
                    alt_text = null
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    var media = Assert.IsType<Media>(createdResult.Value);
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

                var dto = new MediaUpload
                {
                    file = formFile,
                    alt_text = "   "
                };

                using (var context = new DBContext(options))
                {
                    var controller = new MediaController(context, mockEnv.Object);

                    // Act
                    var result = await controller.Upload(dto);

                    // Assert
                    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                    var media = Assert.IsType<Media>(createdResult.Value);
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

        #endregion

        #region DeleteMedia Tests

        [Fact]
        public async Task DeleteMedia_WithValidId_DeletesAndReturnsNoContent()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testMedia = CreateTestMedia(1, "/uploads/test.jpg", "Test");
            using (var context = new DBContext(options))
            {
                context.Medias.Add(testMedia);
                await context.SaveChangesAsync();
            }

            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var mockEnv = CreateMockWebHostEnvironment(tempDir);

                using (var context = new DBContext(options))
                {
                    var controller = new MediaController(context, mockEnv.Object);

                    // Act
                    var result = await controller.DeleteMedia(1);

                    // Assert
                    Assert.IsType<NoContentResult>(result);
                }

                // Verify it was deleted from database
                using (var context = new DBContext(options))
                {
                    var media = await context.Medias.FindAsync(1);
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
                context.Medias.Add(CreateTestMedia(1, "/uploads/test.jpg", "Test"));
                await context.SaveChangesAsync();
            }

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.DeleteMedia(999);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteMedia_WithMultipleMedias_DeletesOnlySpecified()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Medias.AddRange(
                    CreateTestMedia(1, "/uploads/test1.jpg", "Test 1"),
                    CreateTestMedia(2, "/uploads/test2.jpg", "Test 2"),
                    CreateTestMedia(3, "/uploads/test3.jpg", "Test 3")
                );
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                await controller.DeleteMedia(2);
            }

            // Verify only the specified one was deleted
            using (var context = new DBContext(options))
            {
                var medias = await context.Medias.ToListAsync();
                Assert.Equal(2, medias.Count);
                Assert.Null(await context.Medias.FindAsync(2));
                Assert.NotNull(await context.Medias.FindAsync(1));
                Assert.NotNull(await context.Medias.FindAsync(3));
            }
        }

        [Fact]
        public async Task DeleteMedia_WhenFileDoesNotExist_StillDeletesFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testMedia = CreateTestMedia(1, "/uploads/nonexistent.jpg", "Test");
            using (var context = new DBContext(options))
            {
                context.Medias.Add(testMedia);
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act
                var result = await controller.DeleteMedia(1);

                // Assert - should not throw, should delete from database
                Assert.IsType<NoContentResult>(result);
            }

            // Verify it was deleted from database despite file not existing
            using (var context = new DBContext(options))
            {
                var media = await context.Medias.FindAsync(1);
                Assert.Null(media);
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
                context.Medias.Add(CreateTestMedia(1, "/uploads/test.jpg", "Test"));
                await context.SaveChangesAsync();
            }

            var mockEnv = CreateMockWebHostEnvironment();

            using (var context = new DBContext(options))
            {
                var controller = new MediaController(context, mockEnv.Object);

                // Act - we test this indirectly through PutMedia
                var media = new Media { media_id = 1, url = "/uploads/updated.jpg", alt_text = "Updated" };
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
                var controller = new MediaController(context, mockEnv.Object);

                // Act - we test this indirectly through PutMedia
                var media = new Media { media_id = 999, url = "/uploads/test.jpg", alt_text = "Test" };
                var result = await controller.PutMedia(999, media);

                // Assert - if MediaExists returns false, PutMedia should return NotFound
                Assert.IsType<NotFoundResult>(result);
            }
        }

        #endregion
    }
}