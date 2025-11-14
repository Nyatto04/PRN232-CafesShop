using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Dtos.CategoryDtos;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        // (Hàm GetAllCategoriesAsync đã có bên IProductService rồi)

        // Dùng cho Admin quản lý (CRUD)
        Task<BaseResponseDto> AdminGetAllCategoriesAsync();
        Task<BaseResponseDto> GetCategoryByIdAsync(int categoryId);
        Task<BaseResponseDto> CreateCategoryAsync(CreateUpdateCategoryDto categoryDto);
        Task<BaseResponseDto> UpdateCategoryAsync(int categoryId, CreateUpdateCategoryDto categoryDto);
        Task<BaseResponseDto> DeleteCategoryAsync(int categoryId); // Soft delete
    }
}