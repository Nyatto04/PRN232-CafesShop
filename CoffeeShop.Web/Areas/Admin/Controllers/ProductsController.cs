using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Dtos.ProductDtos;
using CoffeeShop.Web.Areas.Admin.Models; 
using Microsoft.AspNetCore.Hosting;    
using System.IO;                       

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApi;
        private readonly IWebHostEnvironment _webHostEnvironment; 

        public ProductsController(IProductApiService productApi, IWebHostEnvironment webHostEnvironment)
        {
            _productApi = productApi;
            _webHostEnvironment = webHostEnvironment;
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
            return View(new CreateProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImageFile.FileName);
                    string productPath = Path.Combine(wwwRootPath, "images", "products");
                    string filePath = Path.Combine(productPath, fileName);

                    if (!Directory.Exists(productPath)) Directory.CreateDirectory(productPath);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }

                    viewModel.ImageUrl = $"/images/products/{fileName}";
                }
                else
                {
                    viewModel.ImageUrl = "/images/placeholder.jpg";
                }

                var result = await _productApi.CreateProductAsync(viewModel);

                if (result.Result == Shared.Dtos.ResultValue.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }

            await LoadCategoriesAsync();
            return View(viewModel); 
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productApi.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var updateDto = new CreateProductViewModel
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