using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApiService;

        public ProductsController(IProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productApiService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(); 
            }

            return View(product); 
        }
    }
}