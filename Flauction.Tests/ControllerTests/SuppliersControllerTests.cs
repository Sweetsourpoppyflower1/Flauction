using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Controllers.newControllers;
using Flauction.Data;
using Flauction.DTOs;
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
        private DbContextOptions<DBContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private Supplier CreateTestSupplier(string id = "1", string email = "supplier@test.com")
        {
            return new Supplier
            {
                Id = id,
                UserName = email,
                Email = email,
                PasswordHash = "hashedpassword123",
                name = "Test Supplier",
                address = "123 Test Street",
                postalcode = "12345",
                country = "Test Country",
                iban = "NL91ABNA0417164300",
                desc = "Test Description"
            };
        }

        private Mock<UserManager<User>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
        }

        [Fact]
        public async Task GetSuppliers_ReturnsAllSuppliers()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Suppliers.AddRange(
                    CreateTestSupplier("1", "supplier1@test.com"),
                    CreateTestSupplier("2", "supplier2@test.com")
                );
                await context.SaveChangesAsync();
            }

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.GetSuppliers();

                // Assert
                var suppliers = Assert.IsAssignableFrom<IEnumerable<Supplier>>(result.Value);
                Assert.Equal(2, suppliers.Count());
            }
        }

        [Fact]
        public async Task GetSuppliers_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.GetSuppliers();

                // Assert
                var suppliers = Assert.IsAssignableFrom<IEnumerable<Supplier>>(result.Value);
                Assert.Empty(suppliers);
            }
        }

        [Fact]
        public async Task GetSupplier_WithValidEmailAndPassword_ReturnsSupplier()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var testSupplier = CreateTestSupplier("1", "supplier@test.com");
            using (var context = new DBContext(options))
            {
                context.Suppliers.Add(testSupplier);
                await context.SaveChangesAsync();
            }

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.GetSupplier("supplier@test.com", "hashedpassword123");

                // Assert
                var actionResult = Assert.IsType<ActionResult<Supplier>>(result);
                var supplier = Assert.IsType<Supplier>(actionResult.Value);
                Assert.Equal("supplier@test.com", supplier.Email);
                Assert.Equal("Test Supplier", supplier.name);
            }
        }

        [Fact]
        public async Task GetSupplier_WithInvalidEmail_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Suppliers.Add(CreateTestSupplier("1", "supplier@test.com"));
                await context.SaveChangesAsync();
            }

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.GetSupplier("nonexistent@test.com", "hashedpassword123");

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task GetSupplier_WithInvalidPassword_ReturnsNotFound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Suppliers.Add(CreateTestSupplier("1", "supplier@test.com"));
                await context.SaveChangesAsync();
            }

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.GetSupplier("supplier@test.com", "wrongpassword");

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task GetSupplier_WithMultipleSuppliers_ReturnsCorrectOne()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new DBContext(options))
            {
                context.Suppliers.AddRange(
                    CreateTestSupplier("1", "supplier1@test.com"),
                    CreateTestSupplier("2", "supplier2@test.com"),
                    CreateTestSupplier("3", "supplier3@test.com")
                );
                await context.SaveChangesAsync();
            }

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.GetSupplier("supplier2@test.com", "hashedpassword123");

                // Assert
                var supplier = Assert.IsType<Supplier>(result.Value);
                Assert.Equal("supplier2@test.com", supplier.Email);
            }
        }

        [Fact]
        public async Task Register_WithValidData_CreatesSupplierAndReturnsCreatedAtAction()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var dto = new SupplierRegisterDTO
            {
                SupplierName = "New Supplier",
                SupplierEmail = "newsupplier@test.com",
                Address = "456 New Street",
                PostalCode = "54321",
                Country = "New Country",
                Iban = "NL91ABNA0417164301",
                Desc = "New Description",
                Password = "SecurePassword123!"
            };

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            // Setup mocks for successful user creation
            mockUserManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockRoleManager
                .Setup(rm => rm.RoleExistsAsync("Supplier"))
                .ReturnsAsync(true);

            mockUserManager
                .Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.Register(dto);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(nameof(SuppliersController.GetSuppliers), createdAtActionResult.ActionName);
                var returnedSupplier = Assert.IsType<Supplier>(createdAtActionResult.Value);
                Assert.Equal("New Supplier", returnedSupplier.name);
                Assert.Equal("456 New Street", returnedSupplier.address);
            }

            // Verify it was saved to the database
            using (var context = new DBContext(options))
            {
                var savedSupplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Email == "newsupplier@test.com");
                Assert.NotNull(savedSupplier);
                Assert.Equal("New Supplier", savedSupplier.name);
            }
        }

        [Fact]
        public async Task Register_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);
                controller.ModelState.AddModelError("SupplierEmail", "Email is required");

                var dto = new SupplierRegisterDTO
                {
                    SupplierName = "New Supplier",
                    SupplierEmail = "",
                    Address = "456 New Street",
                    PostalCode = "54321",
                    Country = "New Country",
                    Iban = "NL91ABNA0417164301",
                    Password = "SecurePassword123!"
                };

                // Act
                var result = await controller.Register(dto);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsConflict()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var dto = new SupplierRegisterDTO
            {
                SupplierName = "New Supplier",
                SupplierEmail = "existing@test.com",
                Address = "456 New Street",
                PostalCode = "54321",
                Country = "New Country",
                Iban = "NL91ABNA0417164301",
                Desc = "New Description",
                Password = "SecurePassword123!"
            };

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            // Setup mock to simulate existing user
            var existingUser = new User { Email = "existing@test.com" };
            mockUserManager
                .Setup(um => um.FindByEmailAsync("existing@test.com"))
                .ReturnsAsync(existingUser);

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.Register(dto);

                // Assert
                var conflictResult = Assert.IsType<ConflictObjectResult>(result);
                Assert.Contains("Email already in use", conflictResult.Value.ToString());
            }
        }

        [Fact]
        public async Task Register_WhenUserCreationFails_ReturnsBadRequest()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var dto = new SupplierRegisterDTO
            {
                SupplierName = "New Supplier",
                SupplierEmail = "newsupplier@test.com",
                Address = "456 New Street",
                PostalCode = "54321",
                Country = "New Country",
                Iban = "NL91ABNA0417164301",
                Desc = "New Description",
                Password = "WeakPassword"
            };

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            mockUserManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Setup mock to simulate password validation failure
            var errors = new[] { new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" } };
            mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.Register(dto);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public async Task Register_WhenRoleDoesNotExist_ReturnsInternalServerError()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var dto = new SupplierRegisterDTO
            {
                SupplierName = "New Supplier",
                SupplierEmail = "newsupplier@test.com",
                Address = "456 New Street",
                PostalCode = "54321",
                Country = "New Country",
                Iban = "NL91ABNA0417164301",
                Desc = "New Description",
                Password = "SecurePassword123!"
            };

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            mockUserManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Setup mock to simulate role does not exist
            mockRoleManager
                .Setup(rm => rm.RoleExistsAsync("Supplier"))
                .ReturnsAsync(false);

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.Register(dto);

                // Assert
                var statusCodeResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal(500, statusCodeResult.StatusCode);
                Assert.Contains("Required role 'Supplier' not found", statusCodeResult.Value.ToString());
            }
        }

        [Fact]
        public async Task Register_WhenAddingRoleFails_ReturnsInternalServerError()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var dto = new SupplierRegisterDTO
            {
                SupplierName = "New Supplier",
                SupplierEmail = "newsupplier@test.com",
                Address = "456 New Street",
                PostalCode = "54321",
                Country = "New Country",
                Iban = "NL91ABNA0417164301",
                Desc = "New Description",
                Password = "SecurePassword123!"
            };

            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            mockUserManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockRoleManager
                .Setup(rm => rm.RoleExistsAsync("Supplier"))
                .ReturnsAsync(true);

            // Setup mock to simulate role assignment failure
            var errors = new[] { new IdentityError { Code = "RoleAssignmentFailed", Description = "Failed to assign role" } };
            mockUserManager
                .Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                var result = await controller.Register(dto);

                // Assert
                var statusCodeResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal(500, statusCodeResult.StatusCode);
                Assert.Contains("Failed to assign role to user", statusCodeResult.Value.ToString());
            }
        }

        [Fact]
        public async Task Register_WithMultipleSuppliers_SavesAllCorrectly()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();

            mockUserManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockRoleManager
                .Setup(rm => rm.RoleExistsAsync("Supplier"))
                .ReturnsAsync(true);

            mockUserManager
                .Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var dto1 = new SupplierRegisterDTO
            {
                SupplierName = "Supplier 1",
                SupplierEmail = "supplier1@test.com",
                Address = "Address 1",
                PostalCode = "11111",
                Country = "Country 1",
                Iban = "IBAN1",
                Desc = "Description 1",
                Password = "Password123!"
            };

            var dto2 = new SupplierRegisterDTO
            {
                SupplierName = "Supplier 2",
                SupplierEmail = "supplier2@test.com",
                Address = "Address 2",
                PostalCode = "22222",
                Country = "Country 2",
                Iban = "IBAN2",
                Desc = "Description 2",
                Password = "Password123!"
            };

            using (var context = new DBContext(options))
            {
                var controller = new SuppliersController(context, mockUserManager.Object, mockRoleManager.Object);

                // Act
                await controller.Register(dto1);
                await controller.Register(dto2);

                // Assert
                var suppliersInDb = await context.Suppliers.ToListAsync();
                Assert.Equal(2, suppliersInDb.Count);
                Assert.Single(suppliersInDb.Where(s => s.name == "Supplier 1"));
                Assert.Single(suppliersInDb.Where(s => s.name == "Supplier 2"));
            }
        }
    }
}