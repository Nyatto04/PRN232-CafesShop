using Shared.Dtos;
using Shared.Dtos.OrderDtos;

namespace CoffeeShop.Web.Services
{
    public interface IOrderApiService
    {
        Task<BaseResponseDto> CreateOrderAsync(CheckoutDto dto);
        Task<BaseResponseDto> GetMyOrdersAsync();
        Task<BaseResponseDto> GetMyOrderDetailsAsync(int orderId);

        Task<BaseResponseDto> GetAllOrdersAsync();
        Task<BaseResponseDto> GetOrderDetailsForAdminAsync(int orderId);
        Task<BaseResponseDto> UpdateOrderStatusAsync(int orderId, string status);
        Task<BaseResponseDto> UpdatePaymentStatusAsync(int orderId, string paymentStatus);
    }
}