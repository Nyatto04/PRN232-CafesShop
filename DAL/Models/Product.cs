using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")] // Kiểu dữ liệu tiền tệ
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string Size { get; set; } // S, M, L

        [MaxLength(500)]
        public string ImageUrl { get; set; } // Link Cloudinary

        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không thể âm")]
        public int Stock { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // --- Foreign Key (Khóa ngoại) ---
        [Required]
        public int CategoryId { get; set; }

        // --- Navigation property ---
        // Mối quan hệ: Một Product thuộc về một Category
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}