using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid

        [MaxLength(500)]
        public string Note { get; set; } // Ghi chú của khách hàng

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // --- Foreign Key ---
        [Required]
        public string UserId { get; set; } // Khóa ngoại tới ApplicationUser

        // --- Thông tin snapshot của khách hàng (lưu lại phòng khi họ đổi profile) ---
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        // --- Navigation Properties ---
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Mối quan hệ: 1 Order có nhiều OrderItems
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }
    }
}