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
        Task<BaseResponseDto> GetProfileAsync(string userId);

        Task<BaseResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto);
    }
}