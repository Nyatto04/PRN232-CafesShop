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

namespace Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<BaseResponseDto> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryDto 
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
            var products = await _context.Products
                .Where(p => p.IsActive && p.Category.IsActive) 
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

        public async Task<BaseResponseDto> GetProductsByCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.IsActive);

            if (category == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy danh mục" };
            }

            var products = await _context.Products
                .Where(p => p.IsActive && p.CategoryId == categoryId && p.Category.IsActive)
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
                .ToListAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Data = products };
        }

        public async Task<BaseResponseDto> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllProductsAsync();
            }

            var searchKeyword = keyword.ToLower().Trim();

            var products = await _context.Products
                .Where(p => p.IsActive && p.Category.IsActive &&
                           (p.ProductName.ToLower().Contains(searchKeyword) ||
                            (p.Description != null && p.Description.ToLower().Contains(searchKeyword))))
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
                .ToListAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Data = products };
        }


        public async Task<BaseResponseDto> CreateProductAsync(CreateUpdateProductDto productDto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId);
            if (!categoryExists)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Danh mục không tồn tại" };
            }

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

            product.IsActive = false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Xóa sản phẩm thành công" };
        }
    }
}