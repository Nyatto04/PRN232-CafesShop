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
        Task<BaseResponseDto> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    }
}