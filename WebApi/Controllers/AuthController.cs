using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.AuthDtos;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto
                {
                    Result = ResultValue.Failed,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result.Result == ResultValue.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto
                {
                    Result = ResultValue.Failed,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result.Result == ResultValue.Success)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (result.Result == ResultValue.Success)
            {
                return Content("<h1>Xác thực email thành công! Bạn có thể đóng tab này.</h1>", "text/html", System.Text.Encoding.UTF8);
            }

            return Content($"<h1>Lỗi: {result.Message}</h1>", "text/html", System.Text.Encoding.UTF8);
        }
    }
}