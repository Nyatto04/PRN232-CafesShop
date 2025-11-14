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
        // Lấy toàn bộ giỏ hàng của user
        Task<BaseResponseDto> GetCartAsync(string userId);

        // Thêm sản phẩm vào giỏ
        Task<BaseResponseDto> AddToCartAsync(string userId, AddCartItemDto cartItemDto);

        // Cập nhật số lượng
        Task<BaseResponseDto> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto updateDto);

        // Xóa 1 sản phẩm khỏi giỏ
        Task<BaseResponseDto> RemoveFromCartAsync(string userId, int cartItemId);
    }
}