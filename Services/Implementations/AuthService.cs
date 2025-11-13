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

            // Tạo ApplicationUser (từ DAL.Models)
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerDto.Email, // Dùng Email làm UserName
                FullName = registerDto.FullName,
                IsActive = false, // Đặt là false, chờ confirm email
                EmailConfirmed = false // Đặt là false, chờ confirm email
            };

            // DÙNG IDENTITY: Tự động băm mật khẩu và lưu user
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Tạo user thất bại: {errors}" };
            }

            // === Tự động tạo Role nếu chưa có ===
            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Customer", Description = "Khách hàng" });
            if (!await _roleManager.RoleExistsAsync("Staff"))
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Staff", Description = "Nhân viên" });
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin", Description = "Quản trị viên" });

            // Gán role "Customer" cho user mới
            await _userManager.AddToRoleAsync(user, "Customer");

            // === Gửi Email Xác thực ===
            // 1. DÙNG IDENTITY: Tạo token xác thực email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // 2. Mã hóa token để an toàn trên URL
            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            // 3. Tạo link callback (trỏ về API của bạn)
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}"; // Tự động lấy http://localhost:5146
            var callbackUrl = $"{baseUrl}/api/Auth/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            // 4. DÙNG MAILKIT: Gửi email
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
            // === KẾT THÚC SỬA ===

            if (result.Succeeded)
            {
                user.IsActive = true; // Kích hoạt tài khoản
                // user.EmailConfirmed đã tự động được set = true bởi Identity
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

            // Kiểm tra user có tồn tại và mật khẩu đúng không
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Email hoặc mật khẩu không chính xác." };
            }

            // Kiểm tra tài khoản đã được kích hoạt/xác thực email chưa
            if (!user.EmailConfirmed)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Tài khoản chưa được xác thực. Vui lòng kiểm tra email." };
            }

            if (!user.IsActive)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Tài khoản của bạn đã bị khóa." };
            }

            // DÙNG IDENTITY: Lấy các Role của user
            var userRoles = await _userManager.GetRolesAsync(user);

            // Tạo Token
            var tokenExpires = DateTime.Now.AddHours(8); // Token hết hạn sau 8 tiếng
            var token = CreateJwtToken(user, userRoles, tokenExpires);

            // Tạo response trả về
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
            // Tạo các claims (thông tin chứa trong token)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("uid", user.Id) // Lưu UserId vào token
            };

            // Thêm các Role vào claims
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Lấy Secret Key từ appsettings.json
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