using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Dtos;
using Shared.Dtos.UserDtos;
using System.Text.Json;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin
    public class UsersController : Controller
    {
        private readonly IAdminApiService _adminApi;

        public UsersController(IAdminApiService adminApi)
        {
            _adminApi = adminApi;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index()
        {
            var result = await _adminApi.GetAllUsersAsync();
            var users = new List<UserDto>();
            if (result.Result == ResultValue.Success && result.Data != null)
            {
                var dataElement = (JsonElement)result.Data;
                users = JsonSerializer.Deserialize<List<UserDto>>(dataElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return View(users);
        }

        // GET: /Admin/Users/Create
        public IActionResult Create()
        {
            // Tạo danh sách Role cho dropdown
            ViewBag.Roles = new SelectList(new[] { "Staff", "Customer" });
            return View();
        }

        // POST: /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminApi.CreateUserAsync(userDto);
                if (result.Result == ResultValue.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message); // Báo lỗi (VD: Email trùng)
            }
            ViewBag.Roles = new SelectList(new[] { "Staff", "Customer" });
            return View(userDto);
        }

        // GET: /Admin/Users/Edit/string-guid
        public async Task<IActionResult> Edit(string id)
        {
            var result = await _adminApi.GetUserByIdAsync(id);
            if (result.Result != ResultValue.Success || result.Data == null)
            {
                return NotFound();
            }

            var dataElement = (JsonElement)result.Data;
            var user = JsonSerializer.Deserialize<UserDto>(dataElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Chuyển từ UserDto sang UpdateUserDto
            var updateDto = new UpdateUserDto
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

            return View(updateDto);
        }

        // POST: /Admin/Users/Edit/string-guid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminApi.UpdateUserAsync(id, userDto);
                if (result.Result == ResultValue.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(userDto);
        }

        // GET: /Admin/Users/Delete/string-guid
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _adminApi.GetUserByIdAsync(id);
            if (result.Result != ResultValue.Success || result.Data == null)
            {
                return NotFound();
            }

            var dataElement = (JsonElement)result.Data;
            var user = JsonSerializer.Deserialize<UserDto>(dataElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(user); // Gửi UserDto đến View
        }

        // POST: /Admin/Users/Delete/string-guid
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _adminApi.DeleteUserAsync(id);
            if (result.Result != ResultValue.Success)
            {
                TempData["Error"] = result.Message; // Báo lỗi (VD: Không thể xóa Admin)
            }
            return RedirectToAction("Index");
        }
    }
}