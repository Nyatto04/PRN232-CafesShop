using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Dtos.UserDtos;

namespace Services.Interfaces
{
    public interface IUserService
    {
        // Lấy profile dựa trên UserId (lấy từ Token)
        Task<BaseResponseDto> GetProfileAsync(string userId);

        // Cập nhật profile
        Task<BaseResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto);
    }
}