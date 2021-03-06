﻿using System;

namespace PipelinedApi.Models
{
    public class StopInfo
    {
        public string StopId { get; set; }
        public string DisplayStopId { get; set; }
        public string ShortName { get; set; }
        public string ShortNameLocalized { get; set; }
        public string FullName { get; set; }
        public string FullNameLocalized { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime LastUpdated { get; set; }

        public OperatorInfo[] Operators { get; set; }
    }

    // _name

    // abrev
    // isParkRide
    // isCycleRide
    // lat
    // long
    // pronunciation
    // text
}
