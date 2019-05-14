using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Caching.Contexts
{
    public interface ICachingContext : IHttpRequestResponseContext
    {
        bool UseCache { get; set; }
        bool FromCache { get; set; }
    }
}
