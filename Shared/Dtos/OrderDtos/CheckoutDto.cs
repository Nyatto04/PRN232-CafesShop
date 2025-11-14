using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.OrderDtos
{
    public class CheckoutDto
    {
        // Thông tin người nhận (cho phép user ghi đè)
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Note { get; set; } // Ghi chú

        [Required]
        public string PaymentMethod { get; set; } = "COD"; // Mặc định là COD
    }
}