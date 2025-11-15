using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.AuthDtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Shared.Dtos;
using Microsoft.AspNetCore.Authorization; 
using Shared.Dtos.UserDtos; 
using System.Text.Json; 

namespace CoffeeShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApi;
        private readonly IUserApiService _userApi;

        public AccountController(IAuthApiService authApi, IUserApiService userApi)
        {
            _authApi = authApi;
            _userApi = userApi; 
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var loginResponse = await _authApi.LoginAsync(loginDto);

            if (loginResponse == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                return View(loginDto);
            }

            await SignInUser(loginResponse);

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
                new Claim("uid", loginResponse.UserId), 
                new Claim("jwtToken", loginResponse.Token)
            };

            foreach (var role in loginResponse.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = loginResponse.Expires
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var result = await _authApi.RegisterAsync(registerDto);

            if (result.Result == ResultValue.Success)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(registerDto);
        }


        [Authorize(Roles = "Customer")] 
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var result = await _userApi.GetProfileAsync();
            if (result.Result != ResultValue.Success || result.Data == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var dataElement = (JsonElement)result.Data;
            var profile = JsonSerializer.Deserialize<ProfileDto>(dataElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var updateDto = new UpdateProfileDto
            {
                FullName = profile.FullName,
                PhoneNumber = profile.PhoneNumber
            };

            return View(updateDto);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateDto);
            }

            var result = await _userApi.UpdateProfileAsync(updateDto);
            if (result.Result != ResultValue.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(updateDto);
            }

            TempData["Success"] = "Cập nhật thông tin thành công!";

            var oldPrincipal = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (oldPrincipal.Principal != null)
            {
                var newClaims = oldPrincipal.Principal.Claims.ToList();
                var nameClaim = newClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                {
                    newClaims[newClaims.IndexOf(nameClaim)] = new Claim(ClaimTypes.Name, updateDto.FullName);
                }

                var newIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(newIdentity),
                    oldPrincipal.Properties); 
            }

            return View(updateDto); 
        }
    }
}