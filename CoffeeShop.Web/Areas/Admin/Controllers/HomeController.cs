using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")] // Chỉ định đây là Controller thuộc Area "Admin"
    [Authorize(Roles = "Admin,Staff")] // Yêu cầu đăng nhập (Admin hoặc Staff)
    public class HomeController : Controller
    {
        // GET: /Admin/Home/Index hoặc /Admin
        public IActionResult Index()
        {
            // Trang Dashboard
            return View();
        }
    }
}