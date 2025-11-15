using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System.Net.Http.Json; 
using System.Text.Json;
using System.Text;

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
            var response = await _httpClient.PostAsJsonAsync("orders", dto);

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }

        public async Task<BaseResponseDto> GetMyOrdersAsync()
        {
            var response = await _httpClient.GetAsync("orders/my-orders");

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }

        public async Task<BaseResponseDto> GetMyOrderDetailsAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"orders/{orderId}");

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }


        public async Task<BaseResponseDto> GetAllOrdersAsync()
        {
            var response = await _httpClient.GetAsync("admin/orders");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
        }

        public async Task<BaseResponseDto> GetOrderDetailsForAdminAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"admin/orders/{orderId}");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
        }

        public async Task<BaseResponseDto> UpdateOrderStatusAsync(int orderId, string status)
        {
            var statusDto = new { Status = status };
            var response = await _httpClient.PutAsJsonAsync($"admin/orders/{orderId}/status", statusDto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> UpdatePaymentStatusAsync(int orderId, string paymentStatus)
        {
            var paymentDto = new { PaymentStatus = paymentStatus };
            var response = await _httpClient.PutAsJsonAsync($"admin/orders/{orderId}/payment", paymentDto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }
    }
}