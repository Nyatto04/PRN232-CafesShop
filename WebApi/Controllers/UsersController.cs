using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.UserDtos;
using System.Security.Claims; // Cần để đọc Token

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // BẮT BUỘC: Ai muốn gọi Controller này đều phải đăng nhập
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Hàm helper để lấy UserId từ JWT Token
        private string GetCurrentUserId()
        {
            // "uid" là claim chúng ta đã thêm vào khi tạo token (trong AuthService)
            return User.FindFirstValue("uid");
        }

        // GET /api/users/profile
        [HttpGet("profile")]
        [Authorize(Roles = "Customer")] // Chỉ Customer mới được gọi
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _userService.GetProfileAsync(userId);

            if (result.Result == ResultValue.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        // PUT /api/users/profile
        [HttpPut("profile")]
        [Authorize(Roles = "Customer")] // Chỉ Customer mới được gọi
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var userId = GetCurrentUserId();
            var result = await _userService.UpdateProfileAsync(userId, updateProfileDto);

            if (result.Result == ResultValue.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}