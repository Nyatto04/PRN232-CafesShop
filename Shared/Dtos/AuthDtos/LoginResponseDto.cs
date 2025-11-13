using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos.AuthDtos
{
    public class LoginResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public string Token { get; set; } // JWT Token
        public DateTime Expires { get; set; } // Thời gian token hết hạn
    }
}