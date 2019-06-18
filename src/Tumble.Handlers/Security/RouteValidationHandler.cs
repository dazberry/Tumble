using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Security
{
    public class MethodAndRoute
    {
        public string Method { get; set; }
        public string Route { get; set; }
    }

    public enum ExitRule { ExitIfMatch, ExitIfNoMatch, NeverExit }

    public class RouteValidationHandler : IPipelineHandler<HttpRequestMessage>
    {
        public IList<MethodAndRoute> MethodsAndRoutes { get; set; }

        public ExitRule ExitRule { get; set; } = ExitRule.ExitIfNoMatch;

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage)
        {
            //var req = httpRequestMessage;
            //req.RequestUri.AbsolutePath
            //req.Method

            await next.Invoke();
        }
    }
}
