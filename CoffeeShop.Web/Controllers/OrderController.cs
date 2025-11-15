using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.OrderDtos;
using Shared.Dtos.UserDtos; 
using System.Security.Claims;
using Shared.Dtos; 
using System.Text.Json; 

namespace CoffeeShop.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderApiService _orderApi;
        private readonly IUserApiService _userApi;

        public OrderController(IOrderApiService orderApi, IUserApiService userApi) 
        {
            _orderApi = orderApi;
            _userApi = userApi; 
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var profileResult = await _userApi.GetProfileAsync();

            var checkoutDto = new CheckoutDto();
            if (profileResult.Result == ResultValue.Success && profileResult.Data != null)
            {
                var dataElement = (JsonElement)profileResult.Data;
                var profile = JsonSerializer.Deserialize<ProfileDto>(dataElement.GetRawText(),
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (profile != null)
                {
                    checkoutDto.FullName = profile.FullName;
                    checkoutDto.Phone = profile.PhoneNumber;
                    checkoutDto.Email = profile.Email;
                }
            }

            return View(checkoutDto);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto checkoutDto)
        {
            if (!ModelState.IsValid)
            {
                return View(checkoutDto); 
            }

            var result = await _orderApi.CreateOrderAsync(checkoutDto);

            if (result.Result != ResultValue.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(checkoutDto);
            }


            var dataElement = (JsonElement)result.Data;
            var orderId = dataElement.GetInt32();

            return RedirectToAction("Success", new { id = orderId });
        }

        [HttpGet]
        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

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

            return View(orders); 
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderApi.GetMyOrderDetailsAsync(id);

            if (result.Result != ResultValue.Success || result.Data == null)
            {
                return RedirectToAction("History"); 
            }

            var dataElement = (JsonElement)result.Data;
            var orderDetails = JsonSerializer.Deserialize<OrderDetailDto>(dataElement.GetRawText(),
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(orderDetails); 
        }
    }
}