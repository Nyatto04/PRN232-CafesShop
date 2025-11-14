using Shared.Dtos;

namespace CoffeeShop.Web.Services
{
    public interface IUserApiService
    {
        Task<BaseResponseDto> GetProfileAsync();
    }
}