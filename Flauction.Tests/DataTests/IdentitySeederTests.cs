using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Flauction.Tests.DataTests
{
    public class IdentitySeederTests
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;

        private void SetupTestEnvironment(Dictionary<string, string> configData)
        {
            var services = new ServiceCollection();

            // Add Logging
            services.AddLogging();

            // In-memory database for DBContext
            services.AddDbContext<DBContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            // Add Identity services with the same in-memory database
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DBContext>();

            // Mock Configuration
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            services.AddSingleton(_configuration);

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task SeedAsync_CreatesRoles_WhenTheyDoNotExist()
        {
            // Arrange
            SetupTestEnvironment(new Dictionary<string, string>());

            // Act
            await IdentitySeeder.SeedAsync(_serviceProvider, _configuration);

            // Assert
            using var scope = _serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            Assert.True(await roleManager.RoleExistsAsync("Admin"));
            Assert.True(await roleManager.RoleExistsAsync("Supplier"));
            Assert.True(await roleManager.RoleExistsAsync("Client"));
        }

        [Fact]
        public async Task SeedAsync_CreatesAdminUserAndAuctionMaster_WhenTheyDoNotExist()
        {
            // Arrange
            var config = new Dictionary<string, string>
            {
                {"AdminUser:Email", "admin@test.com"},
                {"AdminUser:Password", "StrongPassword123!"}
            };
            SetupTestEnvironment(config);

            // Act
            await IdentitySeeder.SeedAsync(_serviceProvider, _configuration);

            // Assert
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

            var adminUser = await userManager.FindByEmailAsync("admin@test.com");
            Assert.NotNull(adminUser);
            Assert.True(await userManager.IsInRoleAsync(adminUser, "Admin"));

            var auctionMaster = await dbContext.AuctionMasters.FindAsync(adminUser.Id);
            Assert.NotNull(auctionMaster);
            Assert.Equal(adminUser.Email, auctionMaster.Email);
        }

        [Fact]
        public async Task SeedAsync_AssignsAdminRole_WhenUserExistsButNotInRole()
        {
            // Arrange
            var config = new Dictionary<string, string>
            {
                {"AdminUser:Email", "admin@test.com"},
                {"AdminUser:Password", "StrongPassword123!"}
            };
            SetupTestEnvironment(config);

            // Pre-seed a user without the Admin role
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var user = new User { UserName = "admin@test.com", Email = "admin@test.com" };
                await userManager.CreateAsync(user, "StrongPassword123!");
            }

            // Act
            await IdentitySeeder.SeedAsync(_serviceProvider, _configuration);

            // Assert
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var adminUser = await userManager.FindByEmailAsync("admin@test.com");
                Assert.NotNull(adminUser);
                Assert.True(await userManager.IsInRoleAsync(adminUser, "Admin"));
            }
        }

        [Fact]
        public async Task SeedAsync_DoesNothing_WhenAdminAndRolesAlreadyExist()
        {
            // Arrange
            var config = new Dictionary<string, string>
            {
                {"AdminUser:Email", "admin@test.com"},
                {"AdminUser:Password", "StrongPassword123!"}
            };
            SetupTestEnvironment(config);

            // Pre-seed everything
            await IdentitySeeder.SeedAsync(_serviceProvider, _configuration);

            // Act again
            await IdentitySeeder.SeedAsync(_serviceProvider, _configuration);

            // Assert - check counts to ensure no duplicates were created
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

            Assert.Single(userManager.Users);
            Assert.Equal(3, roleManager.Roles.Count());
            Assert.Single(dbContext.AuctionMasters);
        }
    }
}