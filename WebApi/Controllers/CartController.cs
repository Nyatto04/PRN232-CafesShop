using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.CartDtos;
using System.Security.Claims; // Cần để đọc Token

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // BẮT BUỘC: Phải đăng nhập và có Role "Customer"
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Hàm helper để lấy UserId từ JWT Token
        private string GetCurrentUserId()
        {
            // "uid" là claim chúng ta đã thêm vào khi tạo token (trong AuthService)
            return User.FindFirstValue("uid");
        }

        // GET: /api/cart
        // Lấy giỏ hàng của tôi
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.GetCartAsync(userId);
            return Ok(result);
        }

        // POST: /api/cart
        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddCartItemDto addDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var userId = GetCurrentUserId();
            var result = await _cartService.AddToCartAsync(userId, addDto);

            if (result.Result != ResultValue.Success)
            {
                // Có thể là 400 (hết hàng) hoặc 404 (không tìm thấy SP)
                return BadRequest(result);
            }
            return Ok(result);
        }

        // PUT: /api/cart/{cartItemId}
        // Cập nhật số lượng của 1 món trong giỏ
        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var userId = GetCurrentUserId();
            var result = await _cartService.UpdateCartItemAsync(userId, cartItemId, updateDto);

            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE: /api/cart/{cartItemId}
        // Xóa 1 món khỏi giỏ hàng
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);

            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}