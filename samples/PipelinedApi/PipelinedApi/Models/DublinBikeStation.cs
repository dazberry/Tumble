using System;
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

        [JsonConverter(typeof(MicrosecondEpochConverter))]        
        public DateTime Last_update { get; set; }
    }

    public class DublinBikeStationPosition
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    public class MicrosecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override bool CanConvert(Type objectType) =>
            objectType == typeof(DateTime);        

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteRawValue($"\"{((DateTime)value).ToUniversalTime().ToString("s")}Z\"");

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            reader.Value != null
                ? _epoch.AddSeconds((long)reader.Value / 1000d)
                : (DateTime?)null;        
    }

}
