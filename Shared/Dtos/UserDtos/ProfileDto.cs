using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos.UserDtos
{
    // DTO này dùng để hiển thị thông tin profile
    public class ProfileDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } // Thêm số điện thoại
    }
}