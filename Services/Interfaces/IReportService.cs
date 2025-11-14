using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;

namespace Services.Interfaces
{
    public interface IReportService
    {
        // Lấy báo cáo doanh thu
        // Nhận vào ngày bắt đầu và ngày kết thúc
        Task<BaseResponseDto> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    }
}