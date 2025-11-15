using Shared.Dtos;
using Shared.Dtos.AuthDtos;

namespace CoffeeShop.Web.Services
{
    public interface IAuthApiService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<BaseResponseDto> RegisterAsync(RegisterDto registerDto);
    }
}