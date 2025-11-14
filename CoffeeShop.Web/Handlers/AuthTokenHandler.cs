using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace CoffeeShop.Web.Handlers
{
    // Lớp này sẽ tự động đính kèm Token vào HttpClient
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Lấy HttpContext hiện tại
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null && httpContext.User.Identity.IsAuthenticated)
            {
                // Lấy "jwtToken" từ Cookie (Claim) mà chúng ta đã lưu lúc Login
                var token = httpContext.User.FindFirstValue("jwtToken");

                if (!string.IsNullOrEmpty(token))
                {
                    // Đính Token vào Header của request
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            // Tiếp tục gửi request đi
            return await base.SendAsync(request, cancellationToken);
        }
    }
}