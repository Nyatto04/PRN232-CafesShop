using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.CategoryDtos;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin được quản lý danh mục
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiService _categoryApi;

        public CategoriesController(ICategoryApiService categoryApi)
        {
            _categoryApi = categoryApi;
        }

        // GET: /Admin/Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryApi.AdminGetAllCategoriesAsync();
            return View(categories);
        }

        // GET: /Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUpdateCategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryApi.CreateCategoryAsync(categoryDto);
                if (result.Result == Shared.Dtos.ResultValue.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message); // Báo lỗi (ví dụ: Tên trùng)
            }
            return View(categoryDto);
        }

        // GET: /Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryApi.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Chuyển từ CategoryDto sang CreateUpdateCategoryDto
            var updateDto = new CreateUpdateCategoryDto
            {
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive
            };

            return View(updateDto);
        }

        // POST: /Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateUpdateCategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryApi.UpdateCategoryAsync(id, categoryDto);
                if (result.Result == Shared.Dtos.ResultValue.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(categoryDto);
        }

        // GET: /Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryApi.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category); // Gửi CategoryDto đến View
        }

        // POST: /Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoryApi.DeleteCategoryAsync(id);
            if (result.Result != Shared.Dtos.ResultValue.Success)
            {
                // Nếu xóa thất bại (ví dụ: còn sản phẩm)
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}