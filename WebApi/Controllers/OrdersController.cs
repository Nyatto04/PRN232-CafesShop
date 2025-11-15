using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System.Security.Claims; 

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")] 
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrdersController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue("uid");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutDto checkoutDto)
        {
            if (!ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var profileResult = await _userService.GetProfileAsync(userId);
                if (profileResult.Result == ResultValue.Success && profileResult.Data is Shared.Dtos.UserDtos.ProfileDto profile)
                {
                    checkoutDto.FullName ??= profile.FullName;
                    checkoutDto.Phone ??= profile.PhoneNumber;
                    checkoutDto.Email ??= profile.Email;
                }
                else
                {
                    return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu checkout không hợp lệ và không thể lấy từ profile" });
                }
            }

            var currentUserId = GetCurrentUserId();
            var result = await _orderService.CreateOrderAsync(currentUserId, checkoutDto);

            if (result.Result != ResultValue.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetCurrentUserId();
            var result = await _orderService.GetMyOrdersAsync(userId);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyOrderDetails(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _orderService.GetMyOrderDetailsAsync(userId, id);

            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}