using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos
{
    public enum ResultValue
    {
        Success = 1,
        Failed = 0,
        NoData = -1,
        ModificationError = -2,
        NoColumnChanged = -3
    }
}