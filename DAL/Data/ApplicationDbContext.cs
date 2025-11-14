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

        // Khai báo bảng giỏ hàng
        public DbSet<CartItem> CartItems { get; set; }

        // <--- THÊM VÀO (1/2) ---
        // Khai báo bảng Order và OrderItem
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        // -----------------------


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Phải gọi base.OnModelCreating(builder) 
            // để các bảng Identity được cấu hình đúng
            base.OnModelCreating(builder);

            // Cấu hình mối quan hệ 1-Nhiều (1 Category có nhiều Product)
            builder.Entity<Product>()
                .HasOne(p => p.Category) // Một Product có một Category
                .WithMany(c => c.Products) // Một Category có nhiều Product
                .HasForeignKey(p => p.CategoryId); // Khóa ngoại là CategoryId


            // Cấu hình mối quan hệ cho CartItem

            // 1 User có nhiều CartItems
            builder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany() // Không cần ICollection<CartItem> trong ApplicationUser
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa User -> Xóa CartItem

            // 1 Product có nhiều CartItems
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany() // Không cần ICollection<CartItem> trong Product
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa Product -> Xóa CartItem


            // Cấu hình cho Order

            // 1 User có nhiều Orders
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany() // Không cần ICollection<Order> trong ApplicationUser
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // QUAN TRỌNG: Không cho xóa User nếu User đó có Order

            // Cấu hình cho OrderItem
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems) // Dùng ICollection<OrderItem> trong Order
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Order -> Xóa OrderItem

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany() // Không cần ICollection<OrderItem> trong Product
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Product nếu đã có người đặt
            // -----------------------
        }
    }
}