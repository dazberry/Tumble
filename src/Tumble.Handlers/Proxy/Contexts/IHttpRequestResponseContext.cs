using System.Net.Http;

namespace Tumble.Handlers.Proxy.Contexts
{
    public interface IHttpRequestResponseContext
    {
        HttpRequestMessage HttpRequestMessage { get; set; }
        HttpResponseMessage HttpResponseMessage { get; set; }
    }
}
