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
using Microsoft.Extensions.Configuration;
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

        // Helper to mock IConfiguration
        private Mock<IConfiguration> MockConfiguration()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:Key"]).Returns("this-is-a-very-long-secret-key-for-jwt-token-generation-that-must-be-at-least-32-bytes");
            mockConfig.Setup(x => x["Jwt:Issuer"]).Returns("Flauction");
            mockConfig.Setup(x => x["Jwt:Audience"]).Returns("FlaucationUsers");
            mockConfig.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("60");
            return mockConfig;
        }

        [Fact]
        public async Task Login_WithValidAdminCredentials_ReturnsOkWithAdminData()
        {
            // Arrange
            var user = new User { Id = "admin-id", Email = "admin@test.com", UserName = "admin@test.com" };
            var mockUserManager = MockUserManager(user);
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            context.AuctionMasters.Add(new AuctionMaster { Id = "admin-id", Email = "admin@test.com" });
            await context.SaveChangesAsync();

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
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
            Assert.NotEmpty(response.Token);
        }

        [Fact]
        public async Task Login_WithValidSupplierCredentials_ReturnsOkWithSupplierData()
        {
            // Arrange
            var user = new User { Id = "supplier-id", Email = "supplier@test.com", UserName = "supplier@test.com" };
            var mockUserManager = MockUserManager(user);
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

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

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
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
            Assert.NotEmpty(response.Token);
        }

        [Fact]
        public async Task Login_WithValidClientCredentials_ReturnsOkWithCompanyData()
        {
            // Arrange
            var user = new User { Id = "client-id", Email = "client@test.com", UserName = "client@test.com" };
            var mockUserManager = MockUserManager(user);
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            context.Companies.Add(new Company
            {
                Id = "client-id",
                Email = "client@test.com",
                name = "Test Company",
                address = "456 Business Ave",
                postalcode = "67890",
                country = "Testland",
                vat = "VAT123",
                iban = "DE89370400440532013000",
                bicswift = "SWIFT123"
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
            var loginRequest = new AuthLoginRequest { Email = "client@test.com", Password = "password" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthLoginResponse>(okResult.Value);
            Assert.Equal("client@test.com", response.Email);
            Assert.Contains("Client", response.Roles);
            var data = Assert.IsType<CompanyDTO>(response.Data);
            Assert.Equal("client-id", data.CompanyID);
            Assert.NotEmpty(response.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = MockUserManager(null); // No user found
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
            var loginRequest = new AuthLoginRequest { Email = "wrong@test.com", Password = "wrong" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var user = new User { Id = "user-id", Email = "user@test.com", UserName = "user@test.com" };
            var mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            
            mockUserManager.Setup(um => um.FindByEmailAsync("user@test.com")).ReturnsAsync(user);
            mockUserManager.Setup(um => um.CheckPasswordAsync(user, "wrongpassword")).ReturnsAsync(false);

            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
            var loginRequest = new AuthLoginRequest { Email = "user@test.com", Password = "wrongpassword" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("email@test.com", null)]
        [InlineData("", "password")]
        [InlineData("email@test.com", "")]
        public async Task Login_WithMissingCredentials_ReturnsBadRequest(string email, string password)
        {
            // Arrange
            var mockUserManager = MockUserManager();
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
            var loginRequest = new AuthLoginRequest { Email = email, Password = password };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithNullRequest_ReturnsBadRequest()
        {
            // Arrange
            var mockUserManager = MockUserManager();
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);

            // Act
            var result = await controller.Login(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_GeneratesValidJwtToken()
        {
            // Arrange
            var user = new User { Id = "user-id", Email = "user@test.com", UserName = "user@test.com" };
            var mockUserManager = MockUserManager(user);
            mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new[] { "Admin" });
            
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            context.AuctionMasters.Add(new AuctionMaster { Id = "user-id", Email = "user@test.com" });
            await context.SaveChangesAsync();

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
            var loginRequest = new AuthLoginRequest { Email = "user@test.com", Password = "password" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthLoginResponse>(okResult.Value);
            Assert.NotNull(response.Token);
            Assert.NotEmpty(response.Token);
        }

        [Fact]
        public async Task Login_WithMissingJwtConfiguration_Returns500Error()
        {
            // Arrange
            var user = new User { Id = "user-id", Email = "user@test.com", UserName = "user@test.com" };
            var mockUserManager = MockUserManager(user);
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:Key"]).Returns((string)null); // Missing JWT Key

            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);
            var loginRequest = new AuthLoginRequest { Email = "user@test.com", Password = "password" };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetCurrentUser_WithValidUser_ReturnsUserData()
        {
            // Arrange
            var user = new User { Id = "user-id", Email = "user@test.com", UserName = "testuser" };
            var mockUserManager = MockUserManager(user);
            mockUserManager.Setup(um => um.FindByIdAsync("user-id")).ReturnsAsync(user);
            mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new[] { "Admin" });

            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);

            // Setup claims for the controller
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "user-id")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = principal };

            // Act
            var result = await controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetCurrentUser_WithoutAuthenticationClaim_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = MockUserManager();
            var mockSignInManager = MockSignInManager(mockUserManager.Object);
            var mockConfig = MockConfiguration();

            using var context = new DBContext(_dbContextOptions);
            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object, context, mockConfig.Object);

            // Setup empty claims
            var identity = new System.Security.Claims.ClaimsIdentity();
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = principal };

            // Act
            var result = await controller.GetCurrentUser();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}