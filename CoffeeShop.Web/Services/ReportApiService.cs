using Shared.Dtos;
using System.Net.Http.Json; // Cần package System.Net.Http.Json
using System.Text.Json;

namespace CoffeeShop.Web.Services
{
    public class ReportApiService : IReportApiService
    {
        private readonly HttpClient _httpClient;

        public ReportApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<BaseResponseDto> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            // Định dạng ngày tháng chuẩn cho URL (YYYY-MM-DD)
            string startDateString = startDate.ToString("yyyy-MM-dd");
            string endDateString = endDate.ToString("yyyy-MM-dd");

            // GET /api/admin/reports/sales?startDate=...&endDate=...
            var response = await _httpClient.GetAsync($"admin/reports/sales?startDate={startDateString}&endDate={endDateString}");

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }
    }
}