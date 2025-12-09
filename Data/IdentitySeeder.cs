using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Flauction.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            string[] roles = new[] { "Admin", "Supplier", "Client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = config["AdminUser:Email"];
            var adminPassword = config["AdminUser:Password"];

            if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var created = await userManager.CreateAsync(adminUser, adminPassword);
                    if (created.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                if (adminUser != null)
                {
                    var existingMaster = await db.AuctionMasters.FindAsync(adminUser.Id);
                    if (existingMaster == null)
                    {
                        var am = new AuctionMaster
                        {
                            Id = adminUser.Id,
                            UserName = adminUser.UserName,
                            Email = adminUser.Email,
                            EmailConfirmed = adminUser.EmailConfirmed
                        };
                        db.AuctionMasters.Add(am);
                        await db.SaveChangesAsync();
                    }
                }
            }

            // Fix existing Supplier users: ensure they have Supplier rows in the database
            var allUsers = await userManager.GetUsersInRoleAsync("Supplier");
            foreach (var user in allUsers)
            {
                var existingSupplier = await db.Suppliers.FindAsync(user.Id);
                if (existingSupplier == null)
                {
                    var supplier = new Supplier
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        name = user.UserName ?? "Supplier",
                        address = "Not provided",
                        postalcode = "0000",
                        country = "Not provided",
                        iban = string.Empty,
                        desc = string.Empty
                    };
                    db.Suppliers.Add(supplier);
                }
            }
            await db.SaveChangesAsync();

            // Fix existing Company users: ensure they have Company rows in the database
            var companyUsers = await userManager.GetUsersInRoleAsync("Client");
            foreach (var user in companyUsers)
            {
                var existingCompany = await db.Companies.FindAsync(user.Id);
                if (existingCompany == null)
                {
                    var company = new Company
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        name = user.UserName ?? "Company",
                        address = "Not provided",
                        postalcode = "0000",
                        country = "Not provided",
                        vat = string.Empty,
                        iban = string.Empty,
                        bicswift = string.Empty
                    };
                    db.Companies.Add(company);
                }
            }
            await db.SaveChangesAsync();
        }
    }
}