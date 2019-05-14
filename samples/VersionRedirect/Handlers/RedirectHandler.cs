using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;
using VersionRedirect.Contexts;

namespace VersionRedirect.Handlers
{
    /// <summary>
    /// Requires VersionContext. Reads VersionNumber, sets HttpContext.Request.Path 
    /// </summary>
    public class RedirectHandler : IPipelineHandler<VersionContext>
    {       
        public async Task InvokeAsync(PipelineDelegate next, VersionContext context)
        {

            if (!string.IsNullOrEmpty(context.VersionNumber))
            {
                var request = context.HttpContext.Request;
                var pathSegments = request.Path.Value.Split('/').ToList();
                if ((pathSegments.Count > 2) && (string.Compare(pathSegments[1], "api", true) == 0))
                {
                    pathSegments.Insert(2, $"v{context.VersionNumber}");
                    var route = string.Join('/', pathSegments);
                    request.Path = route;
                }
            }

            await next.Invoke();
        }
    }
}
