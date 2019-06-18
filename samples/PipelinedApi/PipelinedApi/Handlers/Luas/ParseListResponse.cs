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
    /// Deserialises HttpResponse.Content to LuasLines
    /// <para></para>        
    /// Requires: HttpResponseMessage, IContextWriter&lt;LuasLines&gt;
    /// </summary>    
    public class ParseListResponse : ParseLuasResponse<LuasLines> { }
}
