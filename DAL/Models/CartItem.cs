using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        // Foreign Key to ApplicationUser (Identity uses string Id)
        [Required]
        [MaxLength(450)] // Identity UserId is nvarchar(450)
        public string UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [MaxLength(50)]
        public string? Size { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}

