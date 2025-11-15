using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.CartDtos;

namespace CoffeeShop.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ICartApiService _cartApi;

        public CartController(ICartApiService cartApi)
        {
            _cartApi = cartApi;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = await _cartApi.GetCartAsync();

            ViewBag.TotalAmount = cartItems.Sum(item => item.Subtotal);

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddCartItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }

            var result = await _cartApi.AddToCartAsync(dto);

            if (result.Result != Shared.Dtos.ResultValue.Success)
            {
                TempData["CartError"] = result.Message;
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }

            TempData["CartSuccess"] = "Đã thêm sản phẩm vào giỏ hàng!";

            return RedirectToAction("Index"); 
        }
        [HttpPost]
        public async Task<IActionResult> Remove([FromRoute] int id)
        {
            await _cartApi.RemoveFromCartAsync(id);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return await Remove(id);
            }

            var dto = new UpdateCartItemDto { Quantity = quantity };

            await _cartApi.UpdateCartItemAsync(id, dto);

            return RedirectToAction("Index");
        }
    }
}