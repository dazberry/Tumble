using System.Xml.Serialization;

namespace PipelinedApi.Models
{
    [XmlRoot(ElementName = "stop")]
    public class LuasStopInfo
    {
        [XmlAttribute(AttributeName = "abrev")]
        public string Abrev { get; set; }

        [XmlAttribute(AttributeName = "isParkRide")]
        public string IsParkRide { get; set; }

        [XmlAttribute(AttributeName = "isCycleRide")]
        public string IsCycleRide { get; set; }      
        
        [XmlAttribute(AttributeName = "long")]
        public decimal Longitude { get; set; }

        [XmlAttribute(AttributeName = "lat")]
        public decimal Latitude { get; set; }

        [XmlAttribute(AttributeName = "pronunciation")]        
        public string Pronunciation  { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
