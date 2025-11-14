using Shared.Dtos;
using Shared.Dtos.AuthDtos;

namespace CoffeeShop.Web.Services
{
    public interface IAuthApiService
    {
        // Trả về DTO chứa Token nếu thành công, ngược lại trả về null
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<BaseResponseDto> RegisterAsync(RegisterDto registerDto);
    }
}