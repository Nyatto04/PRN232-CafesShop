using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Web.Controllers
{
    // Controller này dùng chung cho Guest
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApiService;

        public ProductsController(IProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        // GET: /Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Gọi API lấy chi tiết 1 sản phẩm
            var product = await _productApiService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(); // Trả về 404 Not Found nếu API không thấy
            }

            return View(product); // Gửi 1 object ProductDto đến View
        }
    }
}