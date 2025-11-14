using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Dtos.UserDtos;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        Task<BaseResponseDto> GetAllUsersAsync();
        Task<BaseResponseDto> GetUserByIdAsync(string userId);
        Task<BaseResponseDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<BaseResponseDto> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<BaseResponseDto> DeleteUserAsync(string userId);
    }
}