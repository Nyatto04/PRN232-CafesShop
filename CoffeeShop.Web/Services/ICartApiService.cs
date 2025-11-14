using Shared.Dtos;
using Shared.Dtos.CartDtos;

namespace CoffeeShop.Web.Services
{
    public interface ICartApiService
    {
        Task<List<CartItemDto>> GetCartAsync();
        Task<BaseResponseDto> AddToCartAsync(AddCartItemDto dto);
        Task<BaseResponseDto> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto);
        Task<BaseResponseDto> RemoveFromCartAsync(int cartItemId);
    }
}