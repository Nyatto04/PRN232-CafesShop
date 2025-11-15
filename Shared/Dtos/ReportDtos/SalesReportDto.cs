using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos.ReportDtos
{
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; } 
        public int TotalOrders { get; set; } 
        public List<SalesDataPointDto> DailySales { get; set; }
    }
}