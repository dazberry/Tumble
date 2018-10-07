using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PipelinedApi.Models
{
    [XmlRoot(ElementName = "tram")]
    public class LuasTram
    {
        [XmlAttribute(AttributeName = "dueMins")]
        public string DueMins { get; set; }
        [XmlAttribute(AttributeName = "destination")]
        public string Destination { get; set; }
    }

    [XmlRoot(ElementName = "direction")]
    public class LuasDirection
    {
        [XmlElement(ElementName = "tram")]
        public List<LuasTram> Tram { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "stopInfo")]
    public class LuasStop
    {
        [XmlElement(ElementName = "message")]
        public string Message { get; set; }
        [XmlElement(ElementName = "direction")]
        public List<LuasDirection> Direction { get; set; }
        [XmlAttribute(AttributeName = "created")]
        public string Created { get; set; }
        [XmlAttribute(AttributeName = "stop")]
        public string Stop { get; set; }
        [XmlAttribute(AttributeName = "stopAbv")]
        public string StopAbv { get; set; }
    }
}
