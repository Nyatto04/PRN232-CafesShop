using Shared.Dtos;
using Shared.Dtos.CategoryDtos;

namespace CoffeeShop.Web.Services
{
    public interface ICategoryApiService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();

        Task<List<CategoryDto>> AdminGetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<BaseResponseDto> CreateCategoryAsync(CreateUpdateCategoryDto dto);
        Task<BaseResponseDto> UpdateCategoryAsync(int categoryId, CreateUpdateCategoryDto dto);
        Task<BaseResponseDto> DeleteCategoryAsync(int categoryId);
    }
}