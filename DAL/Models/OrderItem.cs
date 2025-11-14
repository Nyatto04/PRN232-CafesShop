using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(200)]
        public string? ProductName { get; set; } // Lưu tên sản phẩm tại thời điểm đặt

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [MaxLength(50)]
        public string? Size { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; } // Price * Quantity

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}

