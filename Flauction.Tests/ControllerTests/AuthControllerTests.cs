using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Flauction.Tests.ControllerTests
{
    public class AuthControllerTests
    {
        private readonly DbContextOptions<DBContext> _dbContextOptions;

        public AuthControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        // Helper to mock UserManager
        private Mock<UserManager<User>> MockUserManager(User user = null)
        {
            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(user);

            userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                       .ReturnsAsync(user != null); // Assume password is correct if user exists

            userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                       .ReturnsAsync(user?.Id switch {
                           "admin-id" => new[] { "Admin" },
                           "supplier-id" => new[] { "Supplier" },
                           "client-id" => new[] { "Client" },
                           _ => Array.Empty<string>()
                       });

            return userManager;
        }

        // Helper to mock SignInManager
        private Mock<SignInManager<User>> MockSignInManager(UserManager<User> userManager)
        {
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            return new Mock<SignInManager<User>>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
        }

        [Fact]
        public async Task Login_WithValidAdminCredentials_ReturnsOkWithAdminData()
        {
            // Arrange
            var user = new User { Id = "admin-id", Email = "admin@test.com", UserName = "admin@test.com" };
            var mockUserManager = MockUserManager(user);
            var mockSignInManager = MockSignInManager(mockUserManager.Object);

            using var context = new DBContext(_dbContextOptions);
            context.AuctionMasters.Add(new AuctionMaster { Id = "admin-id", Email = "admin@test.com" });
            await context.SaveChangesAsync();

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context);
            var loginRequest = new AuthLoginRequest { Email = "admin@test.com", Password = "password" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthLoginResponse>(okResult.Value);
            Assert.Equal("admin@test.com", response.Email);
            Assert.Contains("Admin", response.Roles);
            var data = Assert.IsType<AuctionMasterDTO>(response.Data);
            Assert.Equal("admin-id", data.AuctionMasterId);
        }

        [Fact]
        public async Task Login_WithValidSupplierCredentials_ReturnsOkWithSupplierData()
        {
            // Arrange
            var user = new User { Id = "supplier-id", Email = "supplier@test.com", UserName = "supplier@test.com" };
            var mockUserManager = MockUserManager(user);
            var mockSignInManager = MockSignInManager(mockUserManager.Object);

            using var context = new DBContext(_dbContextOptions);
            context.Suppliers.Add(new Supplier 
            { 
                Id = "supplier-id", 
                Email = "supplier@test.com", 
                name = "Test Supplier",
                address = "123 Test St",
                postalcode = "12345",
                country = "Testland",
                desc = "A test supplier",
                iban = "DE89370400440532013000"
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context);
            var loginRequest = new AuthLoginRequest { Email = "supplier@test.com", Password = "password" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthLoginResponse>(okResult.Value);
            Assert.Equal("supplier@test.com", response.Email);
            Assert.Contains("Supplier", response.Roles);
            var data = Assert.IsType<SupplierDTO>(response.Data);
            Assert.Equal("supplier-id", data.SupplierId);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = MockUserManager(null); // No user found
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context);
            var loginRequest = new AuthLoginRequest { Email = "wrong@test.com", Password = "wrong" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("email@test.com", null)]
        public async Task Login_WithMissingCredentials_ReturnsBadRequest(string email, string password)
        {
            // Arrange
            var mockUserManager = MockUserManager();
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context);
            var loginRequest = new AuthLoginRequest { Email = email, Password = password };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email and password are required.", badRequestResult.Value);
        }
    }
}