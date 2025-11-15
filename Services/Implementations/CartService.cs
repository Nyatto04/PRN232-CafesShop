using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.CartDtos;

namespace Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponseDto> GetCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product) 
                .Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.ProductName,
                    ImageUrl = ci.Product.ImageUrl,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    Size = ci.Size,
                    Subtotal = ci.Product.Price * ci.Quantity
                })
                .ToListAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Data = cartItems };
        }

        public async Task<BaseResponseDto> AddToCartAsync(string userId, AddCartItemDto cartItemDto)
        {
            var product = await _context.Products.FindAsync(cartItemDto.ProductId);
            if (product == null || !product.IsActive)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Sản phẩm không tồn tại" };
            }
            if (product.Stock < cartItemDto.Quantity)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Sản phẩm không đủ hàng" };
            }

            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId &&
                                            ci.ProductId == cartItemDto.ProductId &&
                                            ci.Size == cartItemDto.Size);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItemDto.Quantity;
                _context.CartItems.Update(existingCartItem);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = cartItemDto.ProductId,
                    Quantity = cartItemDto.Quantity,
                    Size = cartItemDto.Size,
                    AddedAt = DateTime.Now
                };
                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();
            return new BaseResponseDto { Result = ResultValue.Success, Message = "Thêm vào giỏ hàng thành công" };
        }

        public async Task<BaseResponseDto> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto updateDto)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null || cartItem.UserId != userId)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy sản phẩm trong giỏ hàng" };
            }

            var product = await _context.Products.FindAsync(cartItem.ProductId);
            if (product.Stock < updateDto.Quantity)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Sản phẩm không đủ hàng" };
            }

            cartItem.Quantity = updateDto.Quantity;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Cập nhật giỏ hàng thành công" };
        }

        public async Task<BaseResponseDto> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null || cartItem.UserId != userId)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy sản phẩm trong giỏ hàng" };
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Xóa sản phẩm khỏi giỏ hàng thành công" };
        }
    }
}