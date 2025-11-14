using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoffeeShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductApiService _productApiService;

        // 1. Tiêm (inject) service gọi API
        public HomeController(IProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        // 2. Sửa hàm Index
        public async Task<IActionResult> Index()
        {
            // Gọi API để lấy danh sách sản phẩm
            var products = await _productApiService.GetProductsAsync();

            // (Chúng ta sẽ lấy Categories sau, giờ cứ hiển thị sản phẩm trước)

            // 3. Gửi danh sách sản phẩm này đến View
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}