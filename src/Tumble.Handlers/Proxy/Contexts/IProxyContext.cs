using Microsoft.AspNetCore.Http;

namespace Tumble.Handlers.Proxy.Contexts
{
    public interface IProxyContext : IHttpRequestResponseContext
    {
        HttpContext HttpContext { get; set; }

    }
}
