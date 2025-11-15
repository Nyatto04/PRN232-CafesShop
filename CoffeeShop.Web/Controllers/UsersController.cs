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
    [Authorize(Roles = "Admin")] 
    public class UsersController : Controller
    {
        private readonly IAdminApiService _adminApi;

        public UsersController(IAdminApiService adminApi)
        {
            _adminApi = adminApi;
        }

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

        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(new[] { "Staff", "Customer" });
            return View();
        }

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
                ModelState.AddModelError(string.Empty, result.Message); 
            }
            ViewBag.Roles = new SelectList(new[] { "Staff", "Customer" });
            return View(userDto);
        }

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

            var updateDto = new UpdateUserDto
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

            return View(updateDto);
        }

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

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _adminApi.DeleteUserAsync(id);
            if (result.Result != ResultValue.Success)
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction("Index");
        }
    }
}