using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.DTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Flauction.Tests.ControllerTests
{
    public class CompaniesControllerTests
    {
        private readonly DbContextOptions<DBContext> _dbContextOptions;

        public CompaniesControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
        }

        [Fact]
        public async Task GetCompanies_WithNoCompanies_ReturnsEmptyList()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetCompanies();

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<Company>>>(result);
            var companies = okResult.Value as List<Company>;
            Assert.NotNull(companies);
            Assert.Empty(companies);
        }

        [Fact]
        public async Task GetCompanies_WithMultipleCompanies_ReturnsAllCompanies()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var company1 = new Company
            {
                Id = "company-1",
                name = "Company One",
                address = "123 Main St",
                postalcode = "12345",
                country = "USA",
                vat = "VAT123",
                iban = "IBAN123",
                bicswift = "SWIFT123"
            };
            var company2 = new Company
            {
                Id = "company-2",
                name = "Company Two",
                address = "456 Oak Ave",
                postalcode = "67890",
                country = "Canada",
                vat = "VAT456",
                iban = "IBAN456",
                bicswift = "SWIFT456"
            };
            context.Companies.AddRange(company1, company2);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetCompanies();

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<Company>>>(result);
            var companies = okResult.Value as List<Company>;
            Assert.NotNull(companies);
            Assert.Equal(2, companies.Count);
            Assert.Contains(companies, c => c.Id == "company-1");
            Assert.Contains(companies, c => c.Id == "company-2");
        }

        [Fact]
        public async Task Register_WithValidData_CreatesUserAndCompany()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            var identityUser = new User { UserName = "test@example.com", Email = "test@example.com" };
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Client")).ReturnsAsync(true);
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Client"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = "test@example.com",
                Password = "Test@123456",
                CompanyName = "Test Company",
                Adress = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Vat = "VAT123",
                Iban = "IBAN123",
                BicSwift = "SWIFT123"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(controller.GetCompanies), createdResult.ActionName);
            var company = Assert.IsType<Company>(createdResult.Value);
            Assert.Equal("Test Company", company.name);
            Assert.Equal("123 Test St", company.address);
            Assert.Equal("12345", company.postalcode);
            Assert.Equal("USA", company.country);
            Assert.Equal("VAT123", company.vat);
            Assert.Equal("IBAN123", company.iban);
            Assert.Equal("SWIFT123", company.bicswift);

            // Verify company was added to database
            mockUserManager.Verify(um => um.FindByEmailAsync("test@example.com"), Times.Once);
            mockUserManager.Verify(um => um.CreateAsync(It.IsAny<User>(), "Test@123456"), Times.Once);
            mockRoleManager.Verify(rm => rm.RoleExistsAsync("Client"), Times.Once);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), "Client"), Times.Once);
        }

        [Fact]
        public async Task Register_WithNullOptionalFields_SetsThemToEmptyString()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Client")).ReturnsAsync(true);
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Client"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = "test@example.com",
                Password = "Test@123456",
                CompanyName = "Test Company",
                Adress = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Vat = null,
                Iban = null,
                BicSwift = null
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var company = Assert.IsType<Company>(createdResult.Value);
            Assert.Equal("", company.vat);
            Assert.Equal("", company.iban);
            Assert.Equal("", company.bicswift);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsConflict()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            var existingUser = new User { UserName = "existing@example.com", Email = "existing@example.com" };
            mockUserManager.Setup(um => um.FindByEmailAsync("existing@example.com")).ReturnsAsync(existingUser);

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = "existing@example.com",
                Password = "Test@123456",
                CompanyName = "Test Company",
                Adress = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Vat = "VAT123",
                Iban = "IBAN123",
                BicSwift = "SWIFT123"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Email already in use.", conflictResult.Value);
            mockUserManager.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Register_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            controller.ModelState.AddModelError("CompanyEmail", "Email is required");

            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = "",
                Password = "Test@123456",
                CompanyName = "Test Company",
                Adress = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Vat = "VAT123",
                Iban = "IBAN123",
                BicSwift = "SWIFT123"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("CompanyEmail"));
        }

        [Fact]
        public async Task Register_WithUserCreationFailure_ReturnsBadRequest()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            var errors = new[] { new IdentityError { Description = "Invalid password" } };
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = "test@example.com",
                Password = "weak",
                CompanyName = "Test Company",
                Adress = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Vat = "VAT123",
                Iban = "IBAN123",
                BicSwift = "SWIFT123"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsType<List<IdentityError>>(badRequestResult.Value);
            Assert.Single(returnedErrors);
            Assert.Equal("Invalid password", returnedErrors[0].Description);
        }

        [Fact]
        public async Task Register_WithMissingClientRole_ReturnsInternalServerError()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Client")).ReturnsAsync(false);

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = "test@example.com",
                Password = "Test@123456",
                CompanyName = "Test Company",
                Adress = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Vat = "VAT123",
                Iban = "IBAN123",
                BicSwift = "SWIFT123"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Required role 'Client' not found.", statusResult.Value);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Register_WithFailureAddingRole_ReturnsInternalServerError()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Client")).ReturnsAsync(true);

            var errors = new[] { new IdentityError { Description = "Failed to add role" } };
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Client"))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = "test@example.com",
                Password = "Test@123456",
                CompanyName = "Test Company",
                Adress = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Vat = "VAT123",
                Iban = "IBAN123",
                BicSwift = "SWIFT123"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Failed to assign role to user.", statusResult.Value);
        }

        [Theory]
        [InlineData("test@example.com", "Test@123456", "Company A", "123 Street", "12345", "USA", "VAT001", "IBAN001", "SWIFT001")]
        [InlineData("another@test.com", "SecurePass123!", "Company B", "456 Avenue", "67890", "Canada", "VAT002", "IBAN002", "SWIFT002")]
        public async Task Register_WithVariousValidInputs_CreatesCompanySuccessfully(
            string email, string password, string name, string address, 
            string postalCode, string country, string vat, string iban, string bicSwift)
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Client")).ReturnsAsync(true);
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Client"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new CompaniesController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new CompanyRegisterDTO
            {
                CompanyEmail = email,
                Password = password,
                CompanyName = name,
                Adress = address,
                PostalCode = postalCode,
                Country = country,
                Vat = vat,
                Iban = iban,
                BicSwift = bicSwift
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var company = Assert.IsType<Company>(createdResult.Value);
            Assert.Equal(name, company.name);
            Assert.Equal(address, company.address);
            Assert.Equal(postalCode, company.postalcode);
            Assert.Equal(country, company.country);
            Assert.Equal(vat, company.vat);
            Assert.Equal(iban, company.iban);
            Assert.Equal(bicSwift, company.bicswift);
        }
    }
}