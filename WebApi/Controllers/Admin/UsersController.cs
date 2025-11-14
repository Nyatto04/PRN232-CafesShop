using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.UserDtos;

namespace WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin được quản lý tài khoản
    public class UsersController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public UsersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: /api/admin/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _adminService.GetAllUsersAsync();
            return Ok(result);
        }

        // GET: /api/admin/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _adminService.GetUserByIdAsync(id);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // POST: /api/admin/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var result = await _adminService.CreateUserAsync(createUserDto);
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }
            return Ok(result); // Trả về 200 OK
        }

        // PUT: /api/admin/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var result = await _adminService.UpdateUserAsync(id, updateUserDto);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE: /api/admin/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}