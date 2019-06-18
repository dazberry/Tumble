using System.Net.Http;
using Tumble.Handlers.Contexts;

namespace Tumble.Handlers.Caching.Contexts
{
    public interface ICacheSettingContext
    {
        bool UseCache { get; set; }
        bool FromCache { get; set; }               
    }
}
