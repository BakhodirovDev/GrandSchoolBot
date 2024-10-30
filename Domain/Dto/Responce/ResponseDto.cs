using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.Responce
{
    public class ResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}
