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
        // === Chức năng cho Guest (Công khai) ===
        Task<BaseResponseDto> GetAllProductsAsync();
        Task<BaseResponseDto> GetProductByIdAsync(int productId);
        Task<BaseResponseDto> GetAllCategoriesAsync();

        // === Chức năng cho Admin (Quản lý) ===
        Task<BaseResponseDto> CreateProductAsync(CreateUpdateProductDto productDto);
        Task<BaseResponseDto> UpdateProductAsync(int productId, CreateUpdateProductDto productDto);
        Task<BaseResponseDto> DeleteProductAsync(int productId); // (Soft delete)

        // (Bạn có thể thêm các hàm CRUD cho Category ở đây nếu muốn)
    }
}