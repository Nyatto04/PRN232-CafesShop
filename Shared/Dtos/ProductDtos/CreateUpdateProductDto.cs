using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.ProductDtos
{
    public class CreateUpdateProductDto
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string Size { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không thể âm")]
        public int Stock { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [Required]
        public int CategoryId { get; set; }
    }
}