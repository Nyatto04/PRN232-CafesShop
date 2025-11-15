using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System.Text.Json;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class OrdersController : Controller
    {
        private readonly IOrderApiService _orderApi;

        public OrdersController(IOrderApiService orderApi)
        {
            _orderApi = orderApi;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _orderApi.GetAllOrdersAsync();
            var orders = new List<OrderDto>();
            if (result.Result == ResultValue.Success && result.Data != null)
            {
                var dataElement = (JsonElement)result.Data;
                orders = JsonSerializer.Deserialize<List<OrderDto>>(dataElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderApi.GetOrderDetailsForAdminAsync(id);
            if (result.Result != ResultValue.Success || result.Data == null)
            {
                return RedirectToAction("Index");
            }

            var dataElement = (JsonElement)result.Data;
            var orderDetails = JsonSerializer.Deserialize<OrderDetailDto>(dataElement.GetRawText(),
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(orderDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                TempData["Error"] = "Trạng thái không hợp lệ";
                return RedirectToAction("Details", new { id });
            }

            var result = await _orderApi.UpdateOrderStatusAsync(id, status);

            if (result.Result != ResultValue.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công!";
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePayment(int id, string paymentStatus)
        {
            if (string.IsNullOrEmpty(paymentStatus))
            {
                TempData["Error"] = "Trạng thái thanh toán không hợp lệ";
                return RedirectToAction("Details", new { id });
            }

            var result = await _orderApi.UpdatePaymentStatusAsync(id, paymentStatus);

            if (result.Result != ResultValue.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "Cập nhật trạng thái thanh toán thành công!";
            }

            return RedirectToAction("Details", new { id });
        }
    }
}