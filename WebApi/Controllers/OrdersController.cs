using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System.Security.Claims; // Cần để đọc Token

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")] // Bắt buộc là Customer
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService; // Dùng để lấy thông tin user

        public OrdersController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        // Hàm helper để lấy UserId từ JWT Token
        private string GetCurrentUserId()
        {
            return User.FindFirstValue("uid");
        }

        // POST: /api/orders
        // Check out (Tạo đơn hàng)
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutDto checkoutDto)
        {
            if (!ModelState.IsValid)
            {
                // Nếu user không điền thông tin, tự động lấy từ profile
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
                // 400 Bad Request (Giỏ hàng rỗng, hết hàng...)
                return BadRequest(result);
            }

            // 200 OK (hoặc 201 Created)
            return Ok(result);
        }

        // GET: /api/orders/my-orders
        // Xem lịch sử đơn hàng của tôi
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetCurrentUserId();
            var result = await _orderService.GetMyOrdersAsync(userId);
            return Ok(result);
        }

        // GET: /api/orders/{id}
        // Xem chi tiết 1 đơn hàng của tôi
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyOrderDetails(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _orderService.GetMyOrderDetailsAsync(userId, id);

            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result); // Không tìm thấy hoặc đơn hàng không phải của user
            }
            return Ok(result);
        }
    }
}