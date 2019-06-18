using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PipelinedApi.Models;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Handlers.Luas
{
    /// <summary>
    /// Request HttpResponseMessage, IContextWriter&lt;LuasStop&gt;
    /// </summary>
    public class ParseStopResponse : ParseLuasResponse<LuasStop> { }
}
