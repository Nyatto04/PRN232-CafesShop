using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin được xem báo cáo
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: /api/admin/reports/sales?startDate=2025-11-01&endDate=2025-11-15
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Đặt giá trị mặc định nếu không truyền
            if (startDate == default) startDate = DateTime.Now.AddDays(-30);
            if (endDate == default) endDate = DateTime.Now;

            var result = await _reportService.GetSalesReportAsync(startDate, endDate);

            if (result.Result == Shared.Dtos.ResultValue.NoData)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}