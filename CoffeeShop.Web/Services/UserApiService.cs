using Shared.Dtos;
using Shared.Dtos.UserDtos;
using System.Text.Json;

namespace CoffeeShop.Web.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<BaseResponseDto> GetProfileAsync()
        {
            // GET /api/users/profile
            // (Token đã được AuthTokenHandler tự động đính kèm)
            var response = await _httpClient.GetAsync("users/profile");

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }
        public async Task<BaseResponseDto> UpdateProfileAsync(UpdateProfileDto dto)
        {
            // PUT /api/users/profile
            var response = await _httpClient.PutAsJsonAsync("users/profile", dto);
            return await response.Content.ReadFromJsonAsync<BaseResponseDto>();
        }
    }
}