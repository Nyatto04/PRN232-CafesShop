using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Dtos.ProductDtos;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<BaseResponseDto> GetAllProductsAsync();
        Task<BaseResponseDto> GetProductByIdAsync(int productId);
        Task<BaseResponseDto> GetAllCategoriesAsync();
        Task<BaseResponseDto> GetProductsByCategoryAsync(int categoryId);
        Task<BaseResponseDto> SearchProductsAsync(string keyword);

        Task<BaseResponseDto> CreateProductAsync(CreateUpdateProductDto productDto);
        Task<BaseResponseDto> UpdateProductAsync(int productId, CreateUpdateProductDto productDto);
        Task<BaseResponseDto> DeleteProductAsync(int productId); 

    }
}