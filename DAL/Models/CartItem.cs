using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [MaxLength(50)]
        public string Size { get; set; } // Lưu lại size user đã chọn

        public DateTime AddedAt { get; set; } = DateTime.Now;

        // --- Foreign Keys (Khóa ngoại) ---

        [Required]
        public string UserId { get; set; } // Khóa ngoại tới ApplicationUser (string)

        [Required]
        public int ProductId { get; set; } // Khóa ngoại tới Product (int)

        // --- Navigation Properties ---

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}