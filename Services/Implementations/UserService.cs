using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.UserDtos;
using System.Security.Claims;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseResponseDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy người dùng" };
            }

            var profileDto = new ProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return new BaseResponseDto
            {
                Result = ResultValue.Success,
                Message = "Lấy thông tin profile thành công",
                Data = profileDto
            };
        }

        public async Task<BaseResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy người dùng" };
            }

            user.FullName = updateProfileDto.FullName;
            user.PhoneNumber = updateProfileDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new BaseResponseDto { Result = ResultValue.Success, Message = "Cập nhật profile thành công" };
            }

            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Cập nhật thất bại: {errors}" };
        }
    }
}