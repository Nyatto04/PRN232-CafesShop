using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Shared.Dtos.ProductDtos;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApi;

        public ProductsController(IProductApiService productApi)
        {
            _productApi = productApi;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productApi.GetProductsAsync();
            return View(products);
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _productApi.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync(); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUpdateProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _productApi.CreateProductAsync(productDto);
                if (result.Result == Shared.Dtos.ResultValue.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }

            await LoadCategoriesAsync();
            return View(productDto);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productApi.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var updateDto = new CreateUpdateProductDto
            {
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Size = product.Size,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId
            };

            await LoadCategoriesAsync();
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateUpdateProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _productApi.UpdateProductAsync(id, productDto);
                if (result.Result == Shared.Dtos.ResultValue.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }

            await LoadCategoriesAsync();
            return View(productDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productApi.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productApi.DeleteProductAsync(id);
            return RedirectToAction("Index");
        }
    }
}