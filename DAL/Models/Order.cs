using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Foreign Key to ApplicationUser (Identity uses string Id)
        [Required]
        [MaxLength(450)] // Identity UserId is nvarchar(450)
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } // Lưu lại thông tin lúc đặt

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }
    }
}

