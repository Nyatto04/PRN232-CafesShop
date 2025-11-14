using Shared.Dtos;
using Shared.Dtos.CategoryDtos;
using Shared.Dtos.ProductDtos;
using System.Text.Json;
namespace CoffeeShop.Web.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            // Sửa thành "products/categories" (viết thường, không có /)
            var response = await _httpClient.GetAsync("products/categories");
            // ... (phần còn lại của hàm)
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

                if (baseResponse.Result == ResultValue.Success && baseResponse.Data != null)
                {
                    var dataElement = (JsonElement)baseResponse.Data;
                    var categories = JsonSerializer.Deserialize<List<CategoryDto>>(dataElement.GetRawText(), JsonOptions());
                    return categories ?? new List<CategoryDto>();
                }
            }
            return new List<CategoryDto>();
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            // Sửa thành "products" (viết thường, không có /)
            var response = await _httpClient.GetAsync("products");
            // ... (phần còn lại của hàm)
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

                if (baseResponse.Result == ResultValue.Success && baseResponse.Data != null)
                {
                    var dataElement = (JsonElement)baseResponse.Data;
                    var products = JsonSerializer.Deserialize<List<ProductDto>>(dataElement.GetRawText(), JsonOptions());
                    return products ?? new List<ProductDto>();
                }
            }
            return new List<ProductDto>();
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            // Sửa thành "products/{productId}" (viết thường, không có /)
            var response = await _httpClient.GetAsync($"products/{productId}");
            // ... (phần còn lại của hàm)
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

                if (baseResponse.Result == ResultValue.Success && baseResponse.Data != null)
                {
                    var dataElement = (JsonElement)baseResponse.Data;
                    var product = JsonSerializer.Deserialize<ProductDto>(dataElement.GetRawText(), JsonOptions());
                    return product;
                }
            }
            return null;
        }

        public async Task<BaseResponseDto> CreateProductAsync(CreateUpdateProductDto dto)
        {
            // POST /api/admin/products
            // (Token Admin đã được AuthTokenHandler tự động đính kèm)
            var response = await _httpClient.PostAsJsonAsync("admin/products", dto);

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
            return baseResponse;
        }

        public async Task<BaseResponseDto> UpdateProductAsync(int productId, CreateUpdateProductDto dto)
        {
            // PUT /api/admin/products/{productId}
            var response = await _httpClient.PutAsJsonAsync($"admin/products/{productId}", dto);

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
            return baseResponse;
        }

        public async Task<BaseResponseDto> DeleteProductAsync(int productId)
        {
            // DELETE /api/admin/products/{productId}
            var response = await _httpClient.DeleteAsync($"admin/products/{productId}");

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
            return baseResponse;
        }
    }
}