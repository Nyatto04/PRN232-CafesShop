using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.ReportDtos;

namespace Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponseDto> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            var inclusiveEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            var completedOrders = await _context.Orders
                .Where(o => o.Status == "Completed" && 
                            o.CreatedAt >= startDate.Date &&
                            o.CreatedAt <= inclusiveEndDate)
                .ToListAsync(); 

            if (!completedOrders.Any())
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không có dữ liệu báo cáo" };
            }

            var dailySales = completedOrders
                .GroupBy(o => o.CreatedAt.Date) 
                .Select(group => new SalesDataPointDto
                {
                    Date = group.Key,
                    Sales = group.Sum(o => o.TotalAmount),
                    OrderCount = group.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            var report = new SalesReportDto
            {
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                TotalSales = completedOrders.Sum(o => o.TotalAmount),
                TotalOrders = completedOrders.Count, 
                DailySales = dailySales 
            };

            return new BaseResponseDto { Result = ResultValue.Success, Data = report };
        }
    }
}