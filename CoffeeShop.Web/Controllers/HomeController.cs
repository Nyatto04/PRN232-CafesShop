using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoffeeShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductApiService _productApiService;

        public HomeController(IProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productApiService.GetProductsAsync();


            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}