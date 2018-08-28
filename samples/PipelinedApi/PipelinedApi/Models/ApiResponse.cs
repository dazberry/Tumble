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

    public class ArrivalInfo
    {
        public DateTime ArrivalTime { get; set; }
        public string DueTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DepartureDueTime { get; set; }
        public DateTime ScheduledArrivalDateTime { get; set; }
        public DateTime ScheduleDepartureDateTime { get; set; }

        public string Destination { get; set; }
        public string DestinationLocalized { get; set; }

        public string Origin { get; set; }
        public string OriginLocalized { get; set; }

        public string Direction { get; set; }
        public string Operator { get; set; }
        public string AdditionalInformation { get; set; }
        public string LowFloorStatus { get; set; }
        public string Route { get; set; }
        public DateTime SourceTimeStamp { get; set; }
        public bool Monitored { get; set; }
    }
}
