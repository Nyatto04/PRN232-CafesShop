using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        // GET: /api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result.Result == Shared.Dtos.ResultValue.NoData)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // GET: /api/products/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _productService.GetAllCategoriesAsync();
            return Ok(result);
        }
    }
}