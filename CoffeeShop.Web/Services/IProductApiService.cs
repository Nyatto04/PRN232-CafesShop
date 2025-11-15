using Shared.Dtos; 
using Shared.Dtos.CategoryDtos;
using Shared.Dtos.ProductDtos;

namespace CoffeeShop.Web.Services
{
    public interface IProductApiService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);

        Task<BaseResponseDto> CreateProductAsync(CreateUpdateProductDto dto);
        Task<BaseResponseDto> UpdateProductAsync(int productId, CreateUpdateProductDto dto);
        Task<BaseResponseDto> DeleteProductAsync(int productId);
    }
}