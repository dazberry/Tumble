using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PipelinedApi.Models
{
    public class ApiResponse<T>
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int NumberOfResults { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StopId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Route { get; set; }

        public DateTime TimeStamp { get; set; }

        public IEnumerable<T> Results { get; set; } = new T[0];
    }
}
