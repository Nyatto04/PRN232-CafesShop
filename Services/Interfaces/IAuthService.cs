using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Dtos.AuthDtos;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<BaseResponseDto> LoginAsync(LoginDto loginDto);
        Task<BaseResponseDto> ConfirmEmailAsync(string userId, string token);

    }
}