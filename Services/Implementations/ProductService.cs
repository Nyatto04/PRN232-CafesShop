using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Shared.Dtos.CategoryDtos;
using Shared.Dtos;
using Shared.Dtos.ProductDtos;
using System.Linq;

namespace Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // === GUEST FUNCTIONS ===

        public async Task<BaseResponseDto> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryDto // Chuyển đổi sang DTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Data = categories };
        }

        public async Task<BaseResponseDto> GetAllProductsAsync()
        {
            // Dùng LINQ để join Product và Category, sau đó chuyển sang DTO
            var products = await _context.Products
                .Where(p => p.IsActive && p.Category.IsActive) // Chỉ lấy sp/danh mục đang active
                .Include(p => p.Category) // Join với bảng Category
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    Size = p.Size,
                    ImageUrl = p.ImageUrl,
                    Stock = p.Stock,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName // Lấy tên từ Category
                })
                .ToListAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Data = products };
        }

        public async Task<BaseResponseDto> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .Where(p => p.IsActive && p.ProductId == productId)
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    Size = p.Size,
                    ImageUrl = p.ImageUrl,
                    Stock = p.Stock,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy sản phẩm" };
            }

            return new BaseResponseDto { Result = ResultValue.Success, Data = product };
        }

        // === ADMIN FUNCTIONS ===

        public async Task<BaseResponseDto> CreateProductAsync(CreateUpdateProductDto productDto)
        {
            // Kiểm tra xem CategoryId có tồn tại không
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId);
            if (!categoryExists)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Danh mục không tồn tại" };
            }

            // Chuyển DTO sang Model (Entity)
            var product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                Size = productDto.Size,
                ImageUrl = productDto.ImageUrl,
                Stock = productDto.Stock,
                IsActive = productDto.IsActive,
                CategoryId = productDto.CategoryId,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Tạo sản phẩm thành công", Data = product };
        }

        public async Task<BaseResponseDto> UpdateProductAsync(int productId, CreateUpdateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy sản phẩm" };
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId);
            if (!categoryExists)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Danh mục không tồn tại" };
            }

            // Cập nhật thông tin từ DTO vào Model
            product.ProductName = productDto.ProductName;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Size = productDto.Size;
            product.ImageUrl = productDto.ImageUrl;
            product.Stock = productDto.Stock;
            product.IsActive = productDto.IsActive;
            product.CategoryId = productDto.CategoryId;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Cập nhật sản phẩm thành công" };
        }

        public async Task<BaseResponseDto> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy sản phẩm" };
            }

            // Soft delete (Xóa mềm): Chỉ đánh dấu IsActive = false
            product.IsActive = false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Xóa sản phẩm thành công" };
        }
    }
}