using Shared.Dtos;

namespace CoffeeShop.Web.Services
{
    public interface IReportApiService
    {
        Task<BaseResponseDto> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    }
}