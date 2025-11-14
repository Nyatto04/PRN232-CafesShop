using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Data
{
    public static class DbSeeder
    {
        // Hàm này sẽ được gọi từ Program.cs
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // Lấy các service cần thiết
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // 1. Tạo các Role (Customer, Staff, Admin) nếu chúng chưa tồn tại
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

            // 2. Tạo tài khoản Admin
            string adminEmail = "admin@coffeeshop.com";
            string adminPassword = "AdminPassword123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // Nếu adminUser chưa tồn tại, chúng ta sẽ tạo mới
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin",
                    IsActive = true,
                    // Quan trọng: Xác thực email luôn để có thể đăng nhập
                    EmailConfirmed = true
                };

                // Tạo user mới với mật khẩu
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                // Gán cả 3 role cho Admin (Admin có thể làm mọi thứ)
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(adminUser, new[] { "Customer", "Staff", "Admin" });
                }
            }

            // === BẠN THÊM ĐOẠN CODE NÀY VÀO ===
            // 3. Tạo 5 tài khoản Staff (nếu chưa tồn tại)
            string staffPassword = "StaffPassword123!"; // Mật khẩu chung cho staff

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
                        EmailConfirmed = true // Xác thực luôn để đăng nhập
                    };

                    var result = await userManager.CreateAsync(staffUser, staffPassword);

                    if (result.Succeeded)
                    {
                        // Gán role Staff (và Customer, để họ xem được shop)
                        await userManager.AddToRolesAsync(staffUser, new[] { "Staff", "Customer" });
                    }
                }
            }
            // === KẾT THÚC PHẦN THÊM VÀO ===
        }
    }
}