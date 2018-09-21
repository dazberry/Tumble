using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelinedApi.Models
{
    public class RouteInfo
    {
        public string Operator { get; set; }
        public string Origin { get; set; }
        public string OriginLocalized { get; set; }
        public string Destination { get; set; }
        public string DestinationLocalized { get; set; }
        public DateTime LastUpdated { get; set; }

        public StopInfo[] Stops { get; set; }
    }
}
