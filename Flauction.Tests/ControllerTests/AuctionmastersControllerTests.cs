using System;
using System.Threading.Tasks;
using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace Flauction.Tests.ControllerTests
{
    public class AuctionmastersControllerTests
    {
        private DbContextOptions<DBContext> CreateNewContextOptions()
        {
            // Create a new in-memory database for each test
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithDto()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testEmail = "test@example.com";
            var testPassword = "password123";

            // Seed the database
            using (var context = new DBContext(options))
            {
                context.AuctionMasters.Add(new AuctionMaster { Id = "master-1", Email = testEmail, PasswordHash = testPassword });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new DBContext(options))
            {
                var controller = new AuctionmastersController(context);
                var loginModel = new AuctionMaster { Email = testEmail, PasswordHash = testPassword };
                var result = await controller.Login(loginModel);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var dto = Assert.IsType<AuctionMasterDTO>(okResult.Value);
                Assert.Equal("master-1", dto.AuctionMasterId);
                Assert.Equal(testEmail, dto.Email);
            }
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testEmail = "test@example.com";

            using (var context = new DBContext(options))
            {
                context.AuctionMasters.Add(new AuctionMaster { Id = "master-1", Email = testEmail, PasswordHash = "correct-password" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new DBContext(options))
            {
                var controller = new AuctionmastersController(context);
                var loginModel = new AuctionMaster { Email = testEmail, PasswordHash = "wrong-password" };
                var result = await controller.Login(loginModel);

                // Assert
                Assert.IsType<UnauthorizedResult>(result.Result);
            }
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DBContext(options);
            var controller = new AuctionmastersController(context);
            var loginModel = new AuctionMaster { Email = "nouser@example.com", PasswordHash = "any-password" };

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("email", null)]
        [InlineData("", "password")]
        [InlineData("email", "")]
        public async Task Login_WithMissingEmailOrPassword_ReturnsBadRequest(string email, string password)
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DBContext(options);
            var controller = new AuctionmastersController(context);
            var loginModel = new AuctionMaster { Email = email, PasswordHash = password };

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email and password are required.", badRequestResult.Value);
        }
    }
}

