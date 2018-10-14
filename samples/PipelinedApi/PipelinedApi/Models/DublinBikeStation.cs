using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PipelinedApi.Models
{    
    public class DublinBikeStation
    {
        public int Number { get; set; }
        public string Contract_name { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DublinBikeStationPosition Position { get; set; }
        public bool Banking { get; set; }
        public bool Bonus { get; set; }
        public int Bike_stands { get; set; }
        public int Available_bike_stands { get; set; }
        public int Available_bikes { get; set; }
        public string Status { get; set; }        

        //todo
        //public DateTime Last_update { get; set; }
    }

    public class DublinBikeStationPosition
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

}
