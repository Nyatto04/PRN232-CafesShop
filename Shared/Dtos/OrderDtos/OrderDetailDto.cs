using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos.OrderDtos
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin người nhận
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Danh sách sản phẩm trong đơn hàng
        public List<OrderItemDto> OrderItems { get; set; }
    }
}