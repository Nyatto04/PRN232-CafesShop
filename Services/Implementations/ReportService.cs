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
            // Điều chỉnh endDate để bao gồm cả ngày (đến 23:59:59)
            var inclusiveEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            // 1. Lấy tất cả đơn hàng đã hoàn thành (Completed) trong khoảng thời gian
            var completedOrders = await _context.Orders
                .Where(o => o.Status == "Completed" && // Chỉ tính đơn đã hoàn thành
                            o.CreatedAt >= startDate.Date &&
                            o.CreatedAt <= inclusiveEndDate)
                .ToListAsync(); // Lấy dữ liệu về memory

            if (!completedOrders.Any())
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không có dữ liệu báo cáo" };
            }

            // 2. Nhóm theo ngày (GroupBy) và tính toán
            var dailySales = completedOrders
                .GroupBy(o => o.CreatedAt.Date) // Nhóm theo ngày
                .Select(group => new SalesDataPointDto
                {
                    Date = group.Key,
                    Sales = group.Sum(o => o.TotalAmount), // Tính tổng doanh thu/ngày
                    OrderCount = group.Count() // Đếm số đơn/ngày
                })
                .OrderBy(d => d.Date)
                .ToList();

            // 3. Tạo DTO kết quả
            var report = new SalesReportDto
            {
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                TotalSales = completedOrders.Sum(o => o.TotalAmount), // Tổng doanh thu
                TotalOrders = completedOrders.Count, // Tổng số đơn
                DailySales = dailySales // Dữ liệu theo ngày
            };

            return new BaseResponseDto { Result = ResultValue.Success, Data = report };
        }
    }
}