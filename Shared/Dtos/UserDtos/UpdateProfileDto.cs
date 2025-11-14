using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.UserDtos
{
    // DTO này dùng để nhận dữ liệu khi user muốn cập nhật
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100)]
        public string FullName { get; set; }

        // Ta sẽ không cho phép đổi Email, nhưng có thể cho đổi SĐT
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }
    }
}