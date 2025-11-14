using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System.Net.Http.Json; // Cần package System.Net.Http.Json
using System.Text.Json;
using System.Text; // Cần cho StringContent (mặc dù đã có Json, nhưng để đây cho rõ)

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

        // === CÁC HÀM CỦA CUSTOMER (Bạn đã có) ===

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

        // === CÁC HÀM ADMIN/STAFF (PHẦN THÊM MỚI) ===

        public async Task<BaseResponseDto> GetAllOrdersAsync()
        {
            // GET /api/admin/orders
            // (Token Admin/Staff đã được AuthTokenHandler tự động đính kèm)
            var response = await _httpClient.GetAsync("admin/orders");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
        }

        public async Task<BaseResponseDto> GetOrderDetailsForAdminAsync(int orderId)
        {
            // GET /api/admin/orders/{id}
            var response = await _httpClient.GetAsync($"admin/orders/{orderId}");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
        }

        public async Task<BaseResponseDto> UpdateOrderStatusAsync(int orderId, string status)
        {
            // PUT /api/admin/orders/{id}/status
            var statusDto = new { Status = status }; // Tạo object tạm
            var response = await _httpClient.PutAsJsonAsync($"admin/orders/{orderId}/status", statusDto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> UpdatePaymentStatusAsync(int orderId, string paymentStatus)
        {
            // PUT /api/admin/orders/{id}/payment
            var paymentDto = new { PaymentStatus = paymentStatus }; // Tạo object tạm
            var response = await _httpClient.PutAsJsonAsync($"admin/orders/{orderId}/payment", paymentDto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }
    }
}