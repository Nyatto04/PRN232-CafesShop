using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System.ComponentModel.DataAnnotations; 
namespace WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")] // Bắt buộc là Admin hoặc Staff
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /api/admin/orders
        // Lấy tất cả đơn hàng (Quản lý)
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }

        // GET: /api/admin/orders/{id}
        // Lấy chi tiết 1 đơn hàng
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var result = await _orderService.GetOrderDetailsForAdminAsync(id);

            // === SỬA LỖI Ở ĐÂY ===
            if (result.Result == ResultValue.NoData)
            { // <-- Sửa từ } thành {
                return NotFound(result);
            } // <-- Thêm }
            return Ok(result);
            // === KẾT THÚC SỬA ===
        }

        // DTO nhỏ để nhận dữ liệu update
        public class UpdateStatusDto
        {
            [Required]
            public string Status { get; set; }
        }

        // PUT: /api/admin/orders/{id}/status
        // Cập nhật trạng thái đơn hàng (VD: Pending -> Confirmed)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Trạng thái không hợp lệ" });
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // DTO nhỏ để nhận dữ liệu update
        public class UpdatePaymentDto
        {
            [Required]
            public string PaymentStatus { get; set; }
        }

        // PUT: /api/admin/orders/{id}/payment
        // Cập nhật trạng thái thanh toán (VD: Unpaid -> Paid)
        [HttpPut("{id}/payment")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Trạng thái thanh toán không hợp lệ" });
            }

            var result = await _orderService.UpdatePaymentStatusAsync(id, paymentDto.PaymentStatus);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}