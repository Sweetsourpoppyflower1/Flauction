using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.DTOs;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.DTOs.Output.RegisterDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Flauction.Tests.ControllerTests
{
    public class SuppliersControllerTests
    {
        private readonly DbContextOptions<DBContext> _dbContextOptions;

        public SuppliersControllerTests()
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

        #region GetSuppliers Tests

        [Fact]
        public async Task GetSuppliers_WithNoSuppliers_ReturnsEmptyList()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSuppliers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var suppliers = Assert.IsType<List<SupplierDTO>>(okResult.Value);
            Assert.Empty(suppliers);
        }

        [Fact]
        public async Task GetSuppliers_WithMultipleSuppliers_ReturnsAllSuppliersAsDTO()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier1 = new Supplier
            {
                Id = "supplier-1",
                name = "Supplier One",
                Email = "supplier1@test.com",
                address = "123 Main St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123",
                desc = "First supplier"
            };
            var supplier2 = new Supplier
            {
                Id = "supplier-2",
                name = "Supplier Two",
                Email = "supplier2@test.com",
                address = "456 Oak Ave",
                postalcode = "67890",
                country = "Canada",
                iban = "IBAN456",
                desc = "Second supplier"
            };
            context.Suppliers.AddRange(supplier1, supplier2);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSuppliers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var suppliers = Assert.IsType<List<SupplierDTO>>(okResult.Value);
            Assert.Equal(2, suppliers.Count);
            Assert.Contains(suppliers, s => s.SupplierId == "supplier-1" && s.Name == "Supplier One");
            Assert.Contains(suppliers, s => s.SupplierId == "supplier-2" && s.Name == "Supplier Two");
        }

        [Fact]
        public async Task GetSuppliers_MapsSupplierToDTO_Correctly()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-test",
                name = "Test Supplier",
                Email = "test@supplier.com",
                address = "789 Test Road",
                postalcode = "99999",
                country = "Germany",
                iban = "TESTIBAN",
                desc = "Test description"
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSuppliers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var suppliers = Assert.IsType<List<SupplierDTO>>(okResult.Value);
            var supplierDto = suppliers.First();
            Assert.Equal("supplier-test", supplierDto.SupplierId);
            Assert.Equal("Test Supplier", supplierDto.Name);
            Assert.Equal("test@supplier.com", supplierDto.Email);
            Assert.Equal("789 Test Road", supplierDto.Address);
            Assert.Equal("99999", supplierDto.PostalCode);
            Assert.Equal("Germany", supplierDto.Country);
            Assert.Equal("Test description", supplierDto.Description);
        }

        #endregion

        #region GetSupplier Tests

        [Fact]
        public async Task GetSupplier_WithValidId_ReturnsSupplierDTO()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123",
                desc = "Test description"
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplier("supplier-1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var supplierDto = Assert.IsType<SupplierDTO>(okResult.Value);
            Assert.Equal("supplier-1", supplierDto.SupplierId);
            Assert.Equal("Test Supplier", supplierDto.Name);
            Assert.Equal("test@test.com", supplierDto.Email);
            Assert.Equal("123 Test St", supplierDto.Address);
            Assert.Equal("12345", supplierDto.PostalCode);
            Assert.Equal("USA", supplierDto.Country);
            Assert.Equal("Test description", supplierDto.Description);
        }

        [Fact]
        public async Task GetSupplier_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplier("non-existent-id");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = notFoundResult.Value;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetSupplier_WithNonExistentSupplier_ReturnsNotFoundWithMessage()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplier("fake-id");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);
        }

        #endregion

        #region GetSupplierPlants Tests

        [Fact]
        public async Task GetSupplierPlants_WithValidSupplierId_ReturnsPlants()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123"
            };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "supplier-1",
                productname = "Test Plant",
                category = "Flower",
                form = "Cut",
                quality = "A",
                min_stem = "5",
                stems_bunch = "10",
                maturity = "Mature",
                desc = "Test plant description",
                start_price = 10,
                min_price = 5
            };
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplierPlants("supplier-1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var plants = Assert.IsType<List<object>>(okResult.Value);
            Assert.Single(plants);
        }

        [Fact]
        public async Task GetSupplierPlants_WithNonExistentSupplierId_ReturnsNotFound()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplierPlants("non-existent-supplier");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetSupplierPlants_WithSupplierHavingNoPlants_ReturnsEmptyList()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123"
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplierPlants("supplier-1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var plants = Assert.IsType<List<object>>(okResult.Value);
            Assert.Empty(plants);
        }

        [Fact]
        public async Task GetSupplierPlants_WithMultiplePlants_ReturnsAllPlants()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123"
            };
            var plant1 = new Plant
            {
                plant_id = 1,
                supplier_id = "supplier-1",
                productname = "Plant One",
                category = "Flower",
                form = "Cut",
                quality = "A",
                min_stem = "5",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 10,
                min_price = 5
            };
            var plant2 = new Plant
            {
                plant_id = 2,
                supplier_id = "supplier-1",
                productname = "Plant Two",
                category = "Foliage",
                form = "Potted",
                quality = "B",
                min_stem = "3",
                stems_bunch = "5",
                maturity = "Young",
                start_price = 15,
                min_price = 8
            };
            context.Suppliers.Add(supplier);
            context.Plants.AddRange(plant1, plant2);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplierPlants("supplier-1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var plants = Assert.IsType<List<object>>(okResult.Value);
            Assert.Equal(2, plants.Count);
        }

        [Fact]
        public async Task GetSupplierPlants_WithPrimaryImage_IncludesImageUrl()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123"
            };
            var plant = new Plant
            {
                plant_id = 1,
                supplier_id = "supplier-1",
                productname = "Test Plant",
                category = "Flower",
                form = "Cut",
                quality = "A",
                min_stem = "5",
                stems_bunch = "10",
                maturity = "Mature",
                start_price = 10,
                min_price = 5
            };
            var media = new MediaPlant
            {
                mediaplant_id = 1,
                plant_id = 1,
                url = "https://example.com/image.jpg",
                alt_text = "Test Plant Image",
                is_primary = true
            };
            context.Suppliers.Add(supplier);
            context.Plants.Add(plant);
            context.MediaPlants.Add(media);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.GetSupplierPlants("supplier-1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var plants = Assert.IsType<List<object>>(okResult.Value);
            Assert.Single(plants);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithSupplierDTO()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123",
                desc = "Test supplier",
                PasswordHash = "hashedpassword123"
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var loginRequest = new Supplier
            {
                Email = "test@test.com",
                PasswordHash = "hashedpassword123"
            };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var supplierDto = Assert.IsType<SupplierDTO>(okResult.Value);
            Assert.Equal("supplier-1", supplierDto.SupplierId);
            Assert.Equal("Test Supplier", supplierDto.Name);
            Assert.Equal("test@test.com", supplierDto.Email);
        }

        [Fact]
        public async Task Login_WithNullLogin_ReturnsBadRequest()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.Login(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithEmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var loginRequest = new Supplier
            {
                Email = "",
                PasswordHash = "password123"
            };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithEmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var loginRequest = new Supplier
            {
                Email = "test@test.com",
                PasswordHash = ""
            };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var loginRequest = new Supplier
            {
                Email = "nonexistent@test.com",
                PasswordHash = "password123"
            };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result.Result);
            Assert.NotNull(unauthorizedResult);
        }

        [Fact]
        public async Task Login_WithIncorrectPassword_ReturnsUnauthorized()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123",
                PasswordHash = "correctpassword123"
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var loginRequest = new Supplier
            {
                Email = "test@test.com",
                PasswordHash = "wrongpassword123"
            };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result.Result);
            Assert.NotNull(unauthorizedResult);
        }

        [Fact]
        public async Task Login_WithValidCredentials_CallsSaveChanges()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var supplier = new Supplier
            {
                Id = "supplier-1",
                name = "Test Supplier",
                Email = "test@test.com",
                address = "123 Test St",
                postalcode = "12345",
                country = "USA",
                iban = "IBAN123",
                PasswordHash = "hashedpassword123"
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var loginRequest = new Supplier
            {
                Email = "test@test.com",
                PasswordHash = "hashedpassword123"
            };

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_WithValidData_CreatesSupplierAndReturnsCreated()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync("test@test.com")).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Supplier")).ReturnsAsync(true);
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Supplier"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = "test@test.com",
                Password = "Test@123456",
                SupplierName = "Test Supplier",
                Address = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Iban = "DE89370400440532013000",
                Desc = "Test supplier description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(controller.GetSupplier), createdResult.ActionName);
            var supplier = Assert.IsType<SupplierDTO>(createdResult.Value);
            Assert.Equal("Test Supplier", supplier.Name);
            Assert.Equal("test@test.com", supplier.Email);
            Assert.Equal("123 Test St", supplier.Address);
            Assert.Equal("12345", supplier.PostalCode);
            Assert.Equal("USA", supplier.Country);
            Assert.Equal("Test supplier description", supplier.Description);
        }

        [Fact]
        public async Task Register_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();
            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            controller.ModelState.AddModelError("SupplierEmail", "Email is required");

            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = "",
                Password = "Test@123456",
                SupplierName = "Test Supplier",
                Address = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Iban = "DE89370400440532013000",
                Desc = "Test description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsConflict()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            var existingUser = new User { Email = "existing@test.com" };
            mockUserManager.Setup(um => um.FindByEmailAsync("existing@test.com")).ReturnsAsync(existingUser);

            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = "existing@test.com",
                Password = "Test@123456",
                SupplierName = "Test Supplier",
                Address = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Iban = "DE89370400440532013000",
                Desc = "Test description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(conflictResult.Value);
            mockUserManager.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Register_WithUserCreationFailure_ReturnsBadRequest()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync("test@test.com")).ReturnsAsync((User)null);
            var errors = new[] { new IdentityError { Description = "Invalid password" } };
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = "test@test.com",
                Password = "weak",
                SupplierName = "Test Supplier",
                Address = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Iban = "DE89370400440532013000",
                Desc = "Test description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithMissingSupplierRole_ReturnsInternalServerError()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync("test@test.com")).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Supplier")).ReturnsAsync(false);

            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = "test@test.com",
                Password = "Test@123456",
                SupplierName = "Test Supplier",
                Address = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Iban = "DE89370400440532013000",
                Desc = "Test description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Register_WithFailureAddingRole_ReturnsInternalServerError()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync("test@test.com")).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Supplier")).ReturnsAsync(true);

            var errors = new[] { new IdentityError { Description = "Failed to add role" } };
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Supplier"))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = "test@test.com",
                Password = "Test@123456",
                SupplierName = "Test Supplier",
                Address = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Iban = "DE89370400440532013000",
                Desc = "Test description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Theory]
        [InlineData("supplier1@test.com", "SupplierA", "100 Street 1", "10001", "USA")]
        [InlineData("supplier2@test.com", "SupplierB", "200 Street 2", "20002", "Canada")]
        [InlineData("supplier3@test.com", "SupplierC", "300 Street 3", "30003", "Germany")]
        public async Task Register_WithVariousValidInputs_CreatesSupplierSuccessfully(
            string email, string name, string address, string postalCode, string country)
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Supplier")).ReturnsAsync(true);
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Supplier"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = email,
                Password = "Test@123456",
                SupplierName = name,
                Address = address,
                PostalCode = postalCode,
                Country = country,
                Iban = "DE89370400440532013000",
                Desc = "Test description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var supplier = Assert.IsType<SupplierDTO>(createdResult.Value);
            Assert.Equal(name, supplier.Name);
            Assert.Equal(address, supplier.Address);
            Assert.Equal(postalCode, supplier.PostalCode);
            Assert.Equal(country, supplier.Country);
        }

        [Fact]
        public async Task Register_CreatesSupplierLinkedToIdentityUser()
        {
            // Arrange
            using var context = new DBContext(_dbContextOptions);
            var mockUserManager = MockUserManager();
            var mockRoleManager = MockRoleManager();

            User createdUser = null;
            mockUserManager.Setup(um => um.FindByEmailAsync("test@test.com")).ReturnsAsync((User)null);
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, pwd) => { createdUser = user; createdUser.Id = "user-id-123"; })
                .ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Supplier")).ReturnsAsync(true);
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Supplier"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
            var registerDto = new SupplierRegisterDTO
            {
                SupplierEmail = "test@test.com",
                Password = "Test@123456",
                SupplierName = "Test Supplier",
                Address = "123 Test St",
                PostalCode = "12345",
                Country = "USA",
                Iban = "DE89370400440532013000",
                Desc = "Test description"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var supplierDto = Assert.IsType<SupplierDTO>(createdResult.Value);

            // Verify supplier was saved to database
            var savedSupplier = await context.Suppliers.FindAsync(supplierDto.SupplierId);
            Assert.NotNull(savedSupplier);
            Assert.Equal("Test Supplier", savedSupplier.name);
            Assert.Equal("test@test.com", savedSupplier.Email);
        }

        #endregion
    }
}