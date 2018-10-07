using System.Xml.Serialization;

namespace PipelinedApi.Models
{

    [XmlRoot(ElementName = "stops")]
    public class LuasLines
    {
        [XmlElement(ElementName = "line")]
        public LuasLine[] Line { get; set; }
    }

    [XmlRoot(ElementName = "line")]
    public class LuasLine
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "stop")]
        public LuasStopInfo[] Stop { get; set; }
    }
}
