using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CategoriesController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _productService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }

            if (result.Data is Shared.Dtos.CategoryDtos.CategoryDto categoryDto && !categoryDto.IsActive)
            {
                return NotFound(new BaseResponseDto 
                { 
                    Result = ResultValue.NoData, 
                    Message = "Không tìm thấy danh mục" 
                });
            }

            return Ok(result);
        }
    }
}

