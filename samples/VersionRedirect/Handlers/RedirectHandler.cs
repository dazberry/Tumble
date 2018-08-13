using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tumble.Core;
using Tumble.Middleware;

namespace VersionRedirect.Handlers
{
    public class RedirectHandler : IPipelineHandler
    {       
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpContext httpContext) && context.GetFirst(out string versionNo))
            {
                if (!string.IsNullOrEmpty(versionNo))
                {
                    var request = httpContext.Request;
                    var pathSegments = request.Path.Value.Split('/').ToList();
                    if ((pathSegments.Count > 2) && (string.Compare(pathSegments[1], "api", true) == 0))
                    {
                        pathSegments.Insert(2, $"v{versionNo}");
                        var route = string.Join('/', pathSegments);
                        request.Path = route;
                    }
                }
            }            
               
            await next.Invoke();
        }
    }
}
