using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.CategoryDtos;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiService _categoryApi;

        public CategoriesController(ICategoryApiService categoryApi)
        {
            _categoryApi = categoryApi;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryApi.AdminGetAllCategoriesAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

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
                ModelState.AddModelError(string.Empty, result.Message); 
            }
            return View(categoryDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryApi.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var updateDto = new CreateUpdateCategoryDto
            {
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive
            };

            return View(updateDto);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryApi.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoryApi.DeleteCategoryAsync(id);
            if (result.Result != Shared.Dtos.ResultValue.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}