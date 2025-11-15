using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.UserDtos;
using System.Security.Claims; 

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue("uid");
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Customer")] 
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

        [HttpPut("profile")]
        [Authorize(Roles = "Customer")] 
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