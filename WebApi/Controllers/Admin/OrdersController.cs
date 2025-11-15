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
    [Authorize(Roles = "Admin,Staff")] 
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var result = await _orderService.GetOrderDetailsForAdminAsync(id);

            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            } 
            return Ok(result);
        }

        public class UpdateStatusDto
        {
            [Required]
            public string Status { get; set; }
        }

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

        public class UpdatePaymentDto
        {
            [Required]
            public string PaymentStatus { get; set; }
        }

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