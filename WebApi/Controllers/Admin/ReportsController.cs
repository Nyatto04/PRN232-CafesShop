using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
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