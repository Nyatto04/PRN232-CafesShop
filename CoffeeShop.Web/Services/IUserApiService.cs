using Shared.Dtos;
using Shared.Dtos.UserDtos; 

namespace CoffeeShop.Web.Services
{
    public interface IUserApiService
    {
        Task<BaseResponseDto> GetProfileAsync();

        Task<BaseResponseDto> UpdateProfileAsync(UpdateProfileDto dto);
    }
}