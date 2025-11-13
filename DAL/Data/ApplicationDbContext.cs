using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    // Kế thừa từ IdentityDbContext với User và Role tùy chỉnh
    // Khóa chính (string) là mặc định của Identity
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Phải gọi base.OnModelCreating(builder) 
            // để các bảng Identity được cấu hình đúng
            base.OnModelCreating(builder);

            // Cấu hình thêm (nếu cần)
        }
    }
}