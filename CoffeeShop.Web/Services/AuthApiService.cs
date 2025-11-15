using Shared.Dtos;
using Shared.Dtos.AuthDtos;
using System.Text;
using System.Text.Json;

namespace CoffeeShop.Web.Services
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebApi");
        }

        private JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("auth/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

                if (baseResponse.Result == ResultValue.Success && baseResponse.Data != null)
                {
                    var dataElement = (JsonElement)baseResponse.Data;
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(dataElement.GetRawText(), JsonOptions());
                    return loginResponse;
                }
            }

            return null;
        }

        public async Task<BaseResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(registerDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("auth/register", jsonContent);

            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            return baseResponse;
        }
    }
}