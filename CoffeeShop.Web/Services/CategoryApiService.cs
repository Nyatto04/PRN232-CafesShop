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

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<BaseResponseDto>("products/categories");
            return await DeserializeData<List<CategoryDto>>(response) ?? new List<CategoryDto>();
        }

        public async Task<List<CategoryDto>> AdminGetAllCategoriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<BaseResponseDto>("admin/categories");
            return await DeserializeData<List<CategoryDto>>(response) ?? new List<CategoryDto>();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            var response = await _httpClient.GetFromJsonAsync<BaseResponseDto>($"admin/categories/{categoryId}");
            return await DeserializeData<CategoryDto>(response);
        }

        public async Task<BaseResponseDto> CreateCategoryAsync(CreateUpdateCategoryDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("admin/categories", dto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> UpdateCategoryAsync(int categoryId, CreateUpdateCategoryDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"admin/categories/{categoryId}", dto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> DeleteCategoryAsync(int categoryId)
        {
            var response = await _httpClient.DeleteAsync($"admin/categories/{categoryId}");
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }
    }
}