using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.CategoryDtos;

namespace WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin được dùng
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /api/admin/categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.AdminGetAllCategoriesAsync();
            return Ok(result);
        }

        // GET: /api/admin/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // POST: /api/admin/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateUpdateCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var result = await _categoryService.CreateCategoryAsync(categoryDto);
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }
            return Ok(result); // Đơn giản trả về 200 OK cho nhanh
        }

        // PUT: /api/admin/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateUpdateCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE: /api/admin/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}