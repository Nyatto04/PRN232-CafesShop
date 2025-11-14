using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization; // Cần để bảo vệ
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.CartDtos;

namespace CoffeeShop.Web.Controllers
{
    // BẮT BUỘC: Chỉ Customer đã đăng nhập mới vào được Controller này
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ICartApiService _cartApi;

        public CartController(ICartApiService cartApi)
        {
            _cartApi = cartApi;
        }

        // GET: /Cart/Index
        // Hiển thị trang giỏ hàng
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Gọi API GET /api/cart
            var cartItems = await _cartApi.GetCartAsync();

            // (Chúng ta sẽ tính tổng tiền ở đây hoặc trong View)
            ViewBag.TotalAmount = cartItems.Sum(item => item.Subtotal);

            return View(cartItems);
        }

        // POST: /Cart/AddToCart
        // Hàm này được gọi từ form trên trang Details (Chi tiết sản phẩm)
        [HttpPost]
        public async Task<IActionResult> AddToCart(AddCartItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ (ví dụ: số lượng < 1)
                // Quay lại trang trước đó (thường là trang Details)
                // (Bạn có thể thêm TempData để báo lỗi)
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }

            // Gọi API POST /api/cart
            var result = await _cartApi.AddToCartAsync(dto);

            if (result.Result != Shared.Dtos.ResultValue.Success)
            {
                // Xử lý lỗi (ví dụ: hết hàng)
                // (Thêm TempData để báo lỗi cho người dùng)
                TempData["CartError"] = result.Message;
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }

            TempData["CartSuccess"] = "Đã thêm sản phẩm vào giỏ hàng!";

            // Thêm thành công, quay về trang sản phẩm
            return RedirectToAction("Index"); // Chuyển đến trang Giỏ hàng
        }

        // POST: /Cart/Remove/5
        // Dùng [FromRoute] để lấy cartItemId từ URL
        [HttpPost]
        public async Task<IActionResult> Remove([FromRoute] int id)
        {
            // Gọi API DELETE /api/cart/{id}
            await _cartApi.RemoveFromCartAsync(id);

            // Quay lại trang giỏ hàng
            return RedirectToAction("Index");
        }

        // POST: /Cart/Update/5
        // Dùng để cập nhật số lượng
        [HttpPost]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            if (quantity <= 0)
            {
                // Nếu số lượng là 0, coi như xóa
                return await Remove(id);
            }

            var dto = new UpdateCartItemDto { Quantity = quantity };

            // Gọi API PUT /api/cart/{id}
            await _cartApi.UpdateCartItemAsync(id, dto);

            return RedirectToAction("Index");
        }
    }
}