using Shared.Dtos;
using Shared.Dtos.UserDtos;

namespace CoffeeShop.Web.Services
{
    public interface IAdminApiService
    {
        Task<BaseResponseDto> GetAllUsersAsync();
        Task<BaseResponseDto> GetUserByIdAsync(string userId);
        Task<BaseResponseDto> CreateUserAsync(CreateUserDto dto);
        Task<BaseResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto);
        Task<BaseResponseDto> DeleteUserAsync(string userId);
    }
}