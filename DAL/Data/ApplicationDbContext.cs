using DAL.Models; // Cần using này để thấy ApplicationUser, Category, Product...
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

        // Khai báo cho EF Core biết chúng ta có 2 bảng mới
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        // ===================================



        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Phải gọi base.OnModelCreating(builder) 
            // để các bảng Identity được cấu hình đúng
            base.OnModelCreating(builder);

            // Cấu hình mối quan hệ 1-Nhiều (1 Category có nhiều Product)
            // (EF Core thường tự phát hiện, nhưng làm rõ thì tốt hơn)
            builder.Entity<Product>()
                .HasOne(p => p.Category) // Một Product có một Category
                .WithMany(c => c.Products) // Một Category có nhiều Product
                .HasForeignKey(p => p.CategoryId); // Khóa ngoại là CategoryId
            // ========================================================
        }
    }
}