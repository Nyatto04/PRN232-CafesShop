using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Dtos.ReportDtos; 
using System.Text.Json;

namespace CoffeeShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class ReportsController : Controller
    {
        private readonly IReportApiService _reportApi;

        public ReportsController(IReportApiService reportApi)
        {
            _reportApi = reportApi;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.Now;
            var start = startDate ?? end.AddDays(-29);

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

            return View(report);
        }
    }
}