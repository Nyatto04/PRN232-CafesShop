using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Cần cho SelectList
using Shared.Dtos.ProductDtos;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin được quản lý sản phẩm
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApi;

        public ProductsController(IProductApiService productApi)
        {
            _productApi = productApi;
        }

        // GET: /Admin/Products
        public async Task<IActionResult> Index()
        {
            // Gọi hàm public GetProductsAsync (vì Admin cũng cần xem)
            var products = await _productApi.GetProductsAsync();
            return View(products);
        }

        // Hàm helper để lấy danh sách Category cho dropdown
        private async Task LoadCategoriesAsync()
        {
            var categories = await _productApi.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
        }

        // GET: /Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync(); // Tải danh mục cho dropdown
            return View();
        }

        // POST: /Admin/Products/Create
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

            await LoadCategoriesAsync(); // Tải lại danh mục nếu có lỗi
            return View(productDto);
        }

        // GET: /Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productApi.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Chuyển từ ProductDto (hiển thị) sang CreateUpdateProductDto (form)
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

        // POST: /Admin/Products/Edit/5
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

        // GET: /Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productApi.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product); // Gửi ProductDto đến View
        }

        // POST: /Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productApi.DeleteProductAsync(id);
            return RedirectToAction("Index");
        }
    }
}