using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System.Net.Http.Json; // Cần package System.Net.Http.Json
using System.Text.Json;

namespace CoffeeShop.Web.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly HttpClient _httpClient;

        public OrderApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<BaseResponseDto> CreateOrderAsync(CheckoutDto dto)
        {
            // POST /api/orders
            // (Token đã được AuthTokenHandler tự động đính kèm)
            var response = await _httpClient.PostAsJsonAsync("orders", dto);

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }
        public async Task<BaseResponseDto> GetMyOrdersAsync()
        {
            // GET /api/orders/my-orders
            // (Token đã được AuthTokenHandler tự động đính kèm)
            var response = await _httpClient.GetAsync("orders/my-orders");

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }

        public async Task<BaseResponseDto> GetMyOrderDetailsAsync(int orderId)
        {
            // GET /api/orders/{orderId}
            var response = await _httpClient.GetAsync($"orders/{orderId}");

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }
    }
}