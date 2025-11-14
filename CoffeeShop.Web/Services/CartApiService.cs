using Shared.Dtos;
using Shared.Dtos.CartDtos;
using System.Net.Http.Json; // <-- Cần package System.Net.Http.Json
using System.Text.Json;

namespace CoffeeShop.Web.Services
{
    public class CartApiService : ICartApiService
    {
        private readonly HttpClient _httpClient;

        public CartApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<CartItemDto>> GetCartAsync()
        {
            // GET /api/cart
            // (Token đã được AuthTokenHandler tự động đính kèm)
            var response = await _httpClient.GetAsync("cart");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
                if (baseResponse.Result == ResultValue.Success && baseResponse.Data != null)
                {
                    var dataElement = (JsonElement)baseResponse.Data;
                    var cart = JsonSerializer.Deserialize<List<CartItemDto>>(dataElement.GetRawText(), JsonOptions());
                    return cart ?? new List<CartItemDto>();
                }
            }
            return new List<CartItemDto>();
        }

        public async Task<BaseResponseDto> AddToCartAsync(AddCartItemDto dto)
        {
            // POST /api/cart
            var response = await _httpClient.PostAsJsonAsync("cart", dto);
            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
            return baseResponse;
        }

        public async Task<BaseResponseDto> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto)
        {
            // PUT /api/cart/{cartItemId}
            var response = await _httpClient.PutAsJsonAsync($"cart/{cartItemId}", dto);
            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
            return baseResponse;
        }

        public async Task<BaseResponseDto> RemoveFromCartAsync(int cartItemId)
        {
            // DELETE /api/cart/{cartItemId}
            var response = await _httpClient.DeleteAsync($"cart/{cartItemId}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
            return baseResponse;
        }
    }
}