using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos.OrderDtos
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } // Tên SP lúc đặt
        public string ImageUrl { get; set; } // Lấy từ Product
        public int Quantity { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; } // Giá lúc đặt
        public decimal Subtotal { get; set; }
    }
}