using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelinedApi.Models
{
    public class ApiResponse<T>
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int NumberOfResults { get; set; }
        public string StopId { get; set; }
        public DateTime TimeStamp { get; set; }

        public IEnumerable<T> Results { get; set; } = new T[0];
    }
}
