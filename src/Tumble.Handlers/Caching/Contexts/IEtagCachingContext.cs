using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Caching.Contexts
{
    public interface IEtagCachingContext : IHttpRequestResponseContext
    {        
        bool FromCache { get; set; }
    }
}
