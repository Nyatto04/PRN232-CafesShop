using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos.ProductDtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }

        // Thông tin Category đi kèm
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}