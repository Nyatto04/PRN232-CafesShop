using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.ProductDtos;

namespace WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")] // Đường dẫn là /api/admin/products
    [ApiController]
    [Authorize(Roles = "Admin")] // BẮT BUỘC: Chỉ có Admin mới được gọi
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // POST: /api/admin/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateUpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var result = await _productService.CreateProductAsync(productDto);

            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }

            // Trả về 201 CreatedAtAction (chuẩn RESTful)
            // Cần hàm GetProductById ở controller public
            return CreatedAtAction(
            "GetProductById", // <-- Sửa ở đây: Dùng tên hàm dạng chuỗi
            new { controller = "Products", id = ((DAL.Models.Product)result.Data).ProductId },
            result);
        }

        // PUT: /api/admin/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateUpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponseDto { Result = ResultValue.Failed, Message = "Dữ liệu không hợp lệ" });
            }

            var result = await _productService.UpdateProductAsync(id, productDto);

            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }
            if (result.Result == ResultValue.Failed)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE: /api/admin/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (result.Result == ResultValue.NoData)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}