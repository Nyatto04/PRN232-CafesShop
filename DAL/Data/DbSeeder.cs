using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Customer", Description = "Khách hàng" });
            }
            if (!await roleManager.RoleExistsAsync("Staff"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Staff", Description = "Nhân viên" });
            }
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin", Description = "Quản trị viên" });
            }

            string adminEmail = "admin@coffeeshop.com";
            string adminPassword = "Admin123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin",
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(adminUser, new[] { "Customer", "Staff", "Admin" });
                }
            }

            string staffPassword = "Staff123"; 

            for (int i = 1; i <= 5; i++)
            {
                string staffEmail = $"staff{i}@coffeeshop.com";
                string staffFullName = $"Staff {i}";

                var staffUser = await userManager.FindByEmailAsync(staffEmail);

                if (staffUser == null)
                {
                    staffUser = new ApplicationUser
                    {
                        UserName = staffEmail,
                        Email = staffEmail,
                        FullName = staffFullName,
                        IsActive = true,
                        EmailConfirmed = true 
                    };

                    var result = await userManager.CreateAsync(staffUser, staffPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRolesAsync(staffUser, new[] { "Staff", "Customer" });
                    }
                }
            }
        }
    }
}