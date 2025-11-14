using Shared.Dtos;
using Shared.Dtos.CategoryDtos;
using System.Net.Http.Json;
using System.Text.Json;

namespace CoffeeShop.Web.Services
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // --- Helper để đọc BaseResponse ---
        private async Task<T> DeserializeData<T>(BaseResponseDto response)
        {
            if (response.Result == ResultValue.Success && response.Data != null)
            {
                var dataElement = (JsonElement)response.Data;
                var data = JsonSerializer.Deserialize<T>(dataElement.GetRawText(), JsonOptions());
                return data;
            }
            return default(T);
        }

        // === Public (Guest) ===
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            // GET /api/products/categories
            var response = await _httpClient.GetFromJsonAsync<BaseResponseDto>("products/categories");
            return await DeserializeData<List<CategoryDto>>(response) ?? new List<CategoryDto>();
        }

        // === Admin ===
        public async Task<List<CategoryDto>> AdminGetAllCategoriesAsync()
        {
            // GET /api/admin/categories
            var response = await _httpClient.GetFromJsonAsync<BaseResponseDto>("admin/categories");
            return await DeserializeData<List<CategoryDto>>(response) ?? new List<CategoryDto>();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            // GET /api/admin/categories/{id}
            var response = await _httpClient.GetFromJsonAsync<BaseResponseDto>($"admin/categories/{categoryId}");
            return await DeserializeData<CategoryDto>(response);
        }

        public async Task<BaseResponseDto> CreateCategoryAsync(CreateUpdateCategoryDto dto)
        {
            // POST /api/admin/categories
            var response = await _httpClient.PostAsJsonAsync("admin/categories", dto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> UpdateCategoryAsync(int categoryId, CreateUpdateCategoryDto dto)
        {
            // PUT /api/admin/categories/{id}
            var response = await _httpClient.PutAsJsonAsync($"admin/categories/{categoryId}", dto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> DeleteCategoryAsync(int categoryId)
        {
            // DELETE /api/admin/categories/{id}
            var response = await _httpClient.DeleteAsync($"admin/categories/{categoryId}");
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }
    }
}