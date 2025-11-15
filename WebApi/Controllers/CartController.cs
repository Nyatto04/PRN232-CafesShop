using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.CartDtos;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue("uid");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.GetCartAsync(userId);
            return Ok(result);
        }

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
                return BadRequest(result);
            }
            return Ok(result);
        }

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