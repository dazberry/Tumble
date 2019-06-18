using System.Net.Http;

namespace Tumble.Handlers.Caching.Contexts
{
    public interface IEtagCachingContext
    {               
        bool FromCache { get; set; }
    }
}
