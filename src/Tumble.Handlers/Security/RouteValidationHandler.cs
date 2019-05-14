using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Security
{
    public class MethodAndRoute
    {
        public string Method { get; set; }
        public string Route { get; set; }
    }

    public enum ExitRule { ExitIfMatch, ExitIfNoMatch, NeverExit }

    public class RouteValidationHandler : IPipelineHandler<IHttpRequestResponseContext>
    {
        public IList<MethodAndRoute> MethodsAndRoutes { get; set; }

        public ExitRule ExitRule { get; set; } = ExitRule.ExitIfNoMatch;

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestResponseContext context)
        {
            var req = context.HttpRequestMessage;
            //req.RequestUri.AbsolutePath
            //req.Method

            await next.Invoke();
        }
    }
}
