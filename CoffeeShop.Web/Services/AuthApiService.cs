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
            // 1. Chuyển object LoginDto thành JSON
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            // 2. Gọi API POST /api/auth/login
            var response = await _httpClient.PostAsync("auth/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

                // Nếu API trả về thành công và có data
                if (baseResponse.Result == ResultValue.Success && baseResponse.Data != null)
                {
                    var dataElement = (JsonElement)baseResponse.Data;
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(dataElement.GetRawText(), JsonOptions());
                    return loginResponse;
                }
            }

            // Trả về null nếu đăng nhập thất bại
            return null;
        }

        // --- ĐÃ THÊM HÀM MỚI NÀY ---
        public async Task<BaseResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Chuyển object RegisterDto thành JSON
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(registerDto),
                Encoding.UTF8,
                "application/json");

            // 2. Gọi API POST /api/auth/register
            var response = await _httpClient.PostAsync("auth/register", jsonContent);

            // 3. Đọc kết quả
            var jsonString = await response.Content.ReadAsStringAsync();
            var baseResponse = JsonSerializer.Deserialize<BaseResponseDto>(jsonString, JsonOptions());

            // Trả về kết quả (thành công hoặc thất bại)
            return baseResponse;
        }
        // -------------------------
    }
}