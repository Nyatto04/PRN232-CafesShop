using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.OrderDtos;
using Shared.Dtos.UserDtos; // Cần để lấy Profile
using System.Security.Claims; // Cần để lấy UserId
using Shared.Dtos; 
using System.Text.Json; 

namespace CoffeeShop.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderApiService _orderApi;
        // SỬA LỖI 1: Đổi IUserService thành IUserApiService
        private readonly IUserApiService _userApi;

        public OrderController(IOrderApiService orderApi, IUserApiService userApi) // <-- Sửa ở đây
        {
            _orderApi = orderApi;
            _userApi = userApi; // <-- Sửa ở đây
        }

        // GET: /Order/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            // Lấy thông tin user (Họ tên, SĐT...) để điền sẵn vào form
            var profileResult = await _userApi.GetProfileAsync();

            var checkoutDto = new CheckoutDto();
            if (profileResult.Result == ResultValue.Success && profileResult.Data != null)
            {
                // Phải Deserialize profile từ JsonElement
                var dataElement = (JsonElement)profileResult.Data;
                var profile = JsonSerializer.Deserialize<ProfileDto>(dataElement.GetRawText(),
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (profile != null)
                {
                    // Điền sẵn thông tin từ profile
                    checkoutDto.FullName = profile.FullName;
                    checkoutDto.Phone = profile.PhoneNumber;
                    checkoutDto.Email = profile.Email;
                }
            }

            return View(checkoutDto);
        }

        // POST: /Order/Checkout
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto checkoutDto)
        {
            if (!ModelState.IsValid)
            {
                return View(checkoutDto); // Quay lại form nếu lỗi
            }

            // Gọi API để tạo đơn hàng
            var result = await _orderApi.CreateOrderAsync(checkoutDto);

            if (result.Result != ResultValue.Success)
            {
                // Nếu thất bại (hết hàng, giỏ rỗng...)
                ModelState.AddModelError(string.Empty, result.Message);
                return View(checkoutDto); // Quay lại form và báo lỗi
            }

            // Đặt hàng thành công!
            // Chuyển hướng đến trang Success và truyền ID đơn hàng

            var dataElement = (JsonElement)result.Data;
            var orderId = dataElement.GetInt32();

            return RedirectToAction("Success", new { id = orderId });
        }

        // GET: /Order/Success/5
        [HttpGet]
        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        // GET: /Order/History
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var result = await _orderApi.GetMyOrdersAsync();

            var orders = new List<OrderDto>();
            if (result.Result == ResultValue.Success && result.Data != null)
            {
                var dataElement = (JsonElement)result.Data;
                orders = JsonSerializer.Deserialize<List<OrderDto>>(dataElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return View(orders); // Gửi List<OrderDto> đến View
        }

        // GET: /Order/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderApi.GetMyOrderDetailsAsync(id);

            if (result.Result != ResultValue.Success || result.Data == null)
            {
                return RedirectToAction("History"); // Không tìm thấy đơn hàng
            }

            var dataElement = (JsonElement)result.Data;
            var orderDetails = JsonSerializer.Deserialize<OrderDetailDto>(dataElement.GetRawText(),
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(orderDetails); // Gửi OrderDetailDto đến View
        }
    }
}