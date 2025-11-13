using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Shared.Dtos
{
    public class BaseResponseDto
    {
        public ResultValue Result { get; set; } = ResultValue.Success;
        public string Message { get; set; } = "Thành công";
        public object Data { get; set; }
    }
}