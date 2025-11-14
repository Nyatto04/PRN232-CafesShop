using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Category
    {
        [Key] // Khóa chính
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        // Một Category sẽ có nhiều Products
        public virtual ICollection<Product> Products { get; set; }

        public Category()
        {
            // Khởi tạo để tránh lỗi null
            Products = new HashSet<Product>();
        }
    }
}