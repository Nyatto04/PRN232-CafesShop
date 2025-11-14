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

        // Khai báo cho EF Core biết chúng ta có các bảng
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
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

            // Cấu hình mối quan hệ Order - OrderItem
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order) // Một OrderItem thuộc về một Order
                .WithMany(o => o.OrderItems) // Một Order có nhiều OrderItem
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Order thì xóa luôn OrderItems

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product) // Một OrderItem tham chiếu một Product
                .WithMany() // Một Product có thể có nhiều OrderItem
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Product nếu đã có OrderItem

            // Cấu hình mối quan hệ Order - ApplicationUser
            builder.Entity<Order>()
                .HasOne(o => o.User) // Một Order thuộc về một User
                .WithMany() // Một User có thể có nhiều Order
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa User nếu đã có Order

            // Cấu hình mối quan hệ CartItem - ApplicationUser
            builder.Entity<CartItem>()
                .HasOne(ci => ci.User) // Một CartItem thuộc về một User
                .WithMany() // Một User có thể có nhiều CartItem
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa User thì xóa luôn CartItems

            // Cấu hình mối quan hệ CartItem - Product
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product) // Một CartItem tham chiếu một Product
                .WithMany() // Một Product có thể có nhiều CartItem
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Product thì xóa luôn CartItems
        }
    }
}