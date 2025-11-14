using Shared.Dtos;
using Shared.Dtos.UserDtos;
using System.Net.Http.Json;
using System.Text.Json;

namespace CoffeeShop.Web.Services
{
    public class AdminApiService : IAdminApiService
    {
        private readonly HttpClient _httpClient;

        public AdminApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<BaseResponseDto> GetAllUsersAsync()
        {
            // GET /api/admin/users
            var response = await _httpClient.GetAsync("admin/users");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
        }

        public async Task<BaseResponseDto> GetUserByIdAsync(string userId)
        {
            // GET /api/admin/users/{id}
            var response = await _httpClient.GetAsync($"admin/users/{userId}");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());
        }

        public async Task<BaseResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            // POST /api/admin/users
            var response = await _httpClient.PostAsJsonAsync("admin/users", dto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto)
        {
            // PUT /api/admin/users/{id}
            var response = await _httpClient.PutAsJsonAsync($"admin/users/{userId}", dto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }

        public async Task<BaseResponseDto> DeleteUserAsync(string userId)
        {
            // DELETE /api/admin/users/{id}
            var response = await _httpClient.DeleteAsync($"admin/users/{userId}");
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }
    }
}