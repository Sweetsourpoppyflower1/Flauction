using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Flauction.Models;
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
        }
    }
}