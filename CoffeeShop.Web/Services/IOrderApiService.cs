using Shared.Dtos;
using Shared.Dtos.OrderDtos;

namespace CoffeeShop.Web.Services
{
    public interface IOrderApiService
    {
        // Trả về OrderId nếu thành công
        Task<BaseResponseDto> CreateOrderAsync(CheckoutDto dto);
        Task<BaseResponseDto> GetMyOrdersAsync();
        Task<BaseResponseDto> GetMyOrderDetailsAsync(int orderId);
    }
}