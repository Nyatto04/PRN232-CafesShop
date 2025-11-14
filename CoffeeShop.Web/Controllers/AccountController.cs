using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.AuthDtos; // <-- Đảm bảo bạn có using này
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies; // Cần cho Cookie
using Shared.Dtos; // <-- Thêm using này (cho ResultValue)

namespace CoffeeShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApi;

        public AccountController(IAuthApiService authApi)
        {
            _authApi = authApi;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            // 1. Gọi API để lấy Token
            var loginResponse = await _authApi.LoginAsync(loginDto);

            if (loginResponse == null)
            {
                // Nếu API trả về null (lỗi 401 Unauthorized)
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                return View(loginDto);
            }

            // 2. LƯU TOKEN VÀO COOKIE
            await SignInUser(loginResponse);

            // 3. Chuyển hướng
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDto loginResponse)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginResponse.FullName),
                new Claim(ClaimTypes.Email, loginResponse.Email),
                new Claim("uid", loginResponse.UserId), // Lưu UserId
                
                // Lưu JWT Token để dùng khi gọi API
                new Claim("jwtToken", loginResponse.Token)
            };

            // Thêm các Role
            foreach (var role in loginResponse.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // (Tùy chọn) Remember me
                IsPersistent = true,
                ExpiresUtc = loginResponse.Expires
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // === ĐÃ THÊM 2 HÀM MỚI NÀY ===

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            // Gọi API đăng ký
            var result = await _authApi.RegisterAsync(registerDto);

            if (result.Result == ResultValue.Success)
            {
                return RedirectToAction("Login");
            }

            // Nếu thất bại (ví dụ: Email tồn tại)
            ModelState.AddModelError(string.Empty, result.Message);
            return View(registerDto);
        }
        // ============================
    }
}