using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Dtos.OrderDtos;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<BaseResponseDto> CreateOrderAsync(string userId, CheckoutDto checkoutDto);
        Task<BaseResponseDto> GetMyOrdersAsync(string userId);
        Task<BaseResponseDto> GetMyOrderDetailsAsync(string userId, int orderId);

        Task<BaseResponseDto> GetAllOrdersAsync();
        Task<BaseResponseDto> GetOrderDetailsForAdminAsync(int orderId);
        Task<BaseResponseDto> UpdateOrderStatusAsync(int orderId, string status);
        Task<BaseResponseDto> UpdatePaymentStatusAsync(int orderId, string paymentStatus);
    }
}