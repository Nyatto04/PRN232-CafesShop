using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Dtos.CartDtos;

namespace Services.Interfaces
{
    public interface ICartService
    {
        Task<BaseResponseDto> GetCartAsync(string userId);

        Task<BaseResponseDto> AddToCartAsync(string userId, AddCartItemDto cartItemDto);

        Task<BaseResponseDto> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto updateDto);

        Task<BaseResponseDto> RemoveFromCartAsync(string userId, int cartItemId);
    }
}