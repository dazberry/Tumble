using Microsoft.AspNetCore.Http;
using Tumble.Middleware;

namespace VersionRedirect.Contexts
{
    public class VersionContext : MiddlewareContext
    {
        public string VersionNumber { get; set; } = "";
    }
}
