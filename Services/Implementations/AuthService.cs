using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.AuthDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IMailService mailService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Đăng ký (Register)
        public async Task<BaseResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Email đã tồn tại" };
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerDto.Email,
                FullName = registerDto.FullName,
                IsActive = false, 
                EmailConfirmed = false 
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Tạo user thất bại: {errors}" };
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Customer", Description = "Khách hàng" });
            if (!await _roleManager.RoleExistsAsync("Staff"))
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Staff", Description = "Nhân viên" });
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin", Description = "Quản trị viên" });

            await _userManager.AddToRoleAsync(user, "Customer");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}"; // Tự động lấy http://localhost:5146
            var callbackUrl = $"{baseUrl}/api/Auth/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            string subject = "Xác nhận tài khoản Coffee Shop";
            string body = $"Chào {user.FullName},<br>Cảm ơn bạn đã đăng ký." +
                          $"Vui lòng xác nhận tài khoản bằng cách <a href='{callbackUrl}'>nhấn vào đây</a>.";

            await _mailService.SendEmailAsync(user.Email, subject, body);

            return new BaseResponseDto
            {
                Result = ResultValue.Success,
                Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực."
            };
        }
        #endregion

        #region Xác thực Email (ĐÃ SỬA)
        public async Task<BaseResponseDto> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Thông tin không hợp lệ" };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Không tìm thấy người dùng" };
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
                return new BaseResponseDto { Result = ResultValue.Success, Message = "Xác thực email thành công!" };
            }

            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Xác thực thất bại: {errors}" };
        }
        #endregion

        #region Đăng nhập (Login)
        public async Task<BaseResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Email hoặc mật khẩu không chính xác." };
            }

            if (!user.EmailConfirmed)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Tài khoản chưa được xác thực. Vui lòng kiểm tra email." };
            }

            if (!user.IsActive)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Tài khoản của bạn đã bị khóa." };
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var tokenExpires = DateTime.Now.AddHours(8); 
            var token = CreateJwtToken(user, userRoles, tokenExpires);

            var loginResponse = new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Roles = userRoles,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = token.ValidTo
            };

            return new BaseResponseDto
            {
                Result = ResultValue.Success,
                Message = "Đăng nhập thành công",
                Data = loginResponse
            };
        }
        #endregion

        #region Hàm Private (Tạo JWT)
        private JwtSecurityToken CreateJwtToken(ApplicationUser user, IList<string> userRoles, DateTime tokenExpires)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("uid", user.Id) 
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: tokenExpires,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
        #endregion
    }
}