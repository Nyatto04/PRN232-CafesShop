using Shared.Dtos;
using Shared.Dtos.UserDtos; 

namespace CoffeeShop.Web.Services
{
    public interface IUserApiService
    {
        Task<BaseResponseDto> GetProfileAsync();

        // Giờ C# đã hiểu "UpdateProfileDto" là gì
        Task<BaseResponseDto> UpdateProfileAsync(UpdateProfileDto dto);
    }
}