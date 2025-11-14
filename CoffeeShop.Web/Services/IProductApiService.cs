using Shared.Dtos; // Cần DTOs từ project Shared
using Shared.Dtos.CategoryDtos;
using Shared.Dtos.ProductDtos;

namespace CoffeeShop.Web.Services
{
    public interface IProductApiService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);
    }
}