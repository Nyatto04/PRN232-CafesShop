using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos.ReportDtos
{
    public class SalesDataPointDto
    {
        public DateTime Date { get; set; }
        public decimal Sales { get; set; } // Doanh thu ngày đó
        public int OrderCount { get; set; } // Số đơn ngày đó
    }
}