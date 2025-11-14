using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.UserDtos;

namespace Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<BaseResponseDto> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedAt = user.CreatedAt,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            return new BaseResponseDto { Result = ResultValue.Success, Data = userDtos };
        }

        public async Task<BaseResponseDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy người dùng" };
            }

            var userDto = new UserDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return new BaseResponseDto { Result = ResultValue.Success, Data = userDto };
        }

        public async Task<BaseResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Kiểm tra Role có hợp lệ không
            if (createUserDto.Role != "Staff" && createUserDto.Role != "Customer")
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Role không hợp lệ. Chỉ chấp nhận 'Staff' hoặc 'Customer'." };
            }

            var userExists = await _userManager.FindByEmailAsync(createUserDto.Email);
            if (userExists != null)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Email đã tồn tại" };
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = createUserDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = createUserDto.Email,
                FullName = createUserDto.FullName,
                PhoneNumber = createUserDto.PhoneNumber,
                IsActive = true,
                EmailConfirmed = true // Admin tạo thì cho confirm luôn
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Tạo user thất bại: {errors}" };
            }

            // Gán role
            await _userManager.AddToRoleAsync(user, createUserDto.Role);
            if (createUserDto.Role == "Staff")
            {
                // Staff cũng là 1 customer
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Tạo tài khoản thành công" };
        }

        public async Task<BaseResponseDto> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy người dùng" };
            }

            user.FullName = updateUserDto.FullName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.IsActive = updateUserDto.IsActive;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Cập nhật thất bại: {errors}" };
            }

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Cập nhật tài khoản thành công" };
        }

        public async Task<BaseResponseDto> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy người dùng" };
            }

            // Không cho xóa chính mình
            // (Bạn có thể lấy currentUserId từ HttpContextAccessor nếu inject vào)

            // Không cho xóa Admin
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Không thể xóa tài khoản Admin" };
            }

            // Dùng xóa vĩnh viễn (hoặc bạn có thể chỉ set IsActive = false)
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Xóa thất bại: {errors}" };
            }

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Xóa tài khoản thành công" };
        }
    }
}