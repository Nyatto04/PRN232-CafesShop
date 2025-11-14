using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.CategoryDtos;

namespace Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả (cho Admin, bao gồm cả cái đã Inactive)
        public async Task<BaseResponseDto> AdminGetAllCategoriesAsync()
        {
            var categories = await _context.Categories
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

        public async Task<BaseResponseDto> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy danh mục" };
            }
            return new BaseResponseDto { Result = ResultValue.Success, Data = category };
        }

        public async Task<BaseResponseDto> CreateCategoryAsync(CreateUpdateCategoryDto categoryDto)
        {
            // Kiểm tra trùng tên
            var existing = await _context.Categories
                                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryDto.CategoryName.ToLower());
            if (existing != null)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Tên danh mục đã tồn tại" };
            }

            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Tạo danh mục thành công", Data = category };
        }

        public async Task<BaseResponseDto> UpdateCategoryAsync(int categoryId, CreateUpdateCategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy danh mục" };
            }

            // Kiểm tra trùng tên (nhưng trừ chính nó ra)
            var existing = await _context.Categories
                                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryDto.CategoryName.ToLower()
                                                        && c.CategoryId != categoryId);
            if (existing != null)
            {
                return new BaseResponseDto { Result = ResultValue.Failed, Message = "Tên danh mục đã tồn tại" };
            }

            category.CategoryName = categoryDto.CategoryName;
            category.Description = categoryDto.Description;
            category.IsActive = categoryDto.IsActive;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Cập nhật danh mục thành công" };
        }

        public async Task<BaseResponseDto> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy danh mục" };
            }

            // Kiểm tra xem có sản phẩm nào đang dùng danh mục này không
            var productCount = await _context.Products.CountAsync(p => p.CategoryId == categoryId && p.IsActive);
            if (productCount > 0)
            {
                return new BaseResponseDto
                {
                    Result = ResultValue.Failed,
                    Message = $"Không thể xóa. Có {productCount} sản phẩm đang hoạt động thuộc danh mục này."
                };
            }

            // Xóa mềm (Soft delete)
            category.IsActive = false;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Xóa (ẩn) danh mục thành công" };
        }
    }
}