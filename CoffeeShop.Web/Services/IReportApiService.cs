using Shared.Dtos;

namespace CoffeeShop.Web.Services
{
    public interface IReportApiService
    {
        // Token sẽ được gửi tự động bởi AuthTokenHandler
        Task<BaseResponseDto> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    }
}