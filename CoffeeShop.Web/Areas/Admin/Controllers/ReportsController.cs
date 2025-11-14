using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Dtos.ReportDtos; // Cần DTOs
using System.Text.Json;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin
    public class ReportsController : Controller
    {
        private readonly IReportApiService _reportApi;

        public ReportsController(IReportApiService reportApi)
        {
            _reportApi = reportApi;
        }

        // GET: /Admin/Reports/Index
        // GET: /Admin/Reports?startDate=...&endDate=...
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            // Đặt giá trị mặc định (30 ngày gần nhất)
            var end = endDate ?? DateTime.Now;
            var start = startDate ?? end.AddDays(-29);

            // Truyền ngày đã chọn ra View để hiển thị lại trên form
            ViewBag.StartDate = start;
            ViewBag.EndDate = end;

            var result = await _reportApi.GetSalesReportAsync(start, end);

            SalesReportDto report = null;
            if (result.Result == ResultValue.Success && result.Data != null)
            {
                var dataElement = (JsonElement)result.Data;
                report = JsonSerializer.Deserialize<SalesReportDto>(dataElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // Gửi model (report) đến View
            return View(report);
        }
    }
}