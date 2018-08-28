using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

    public class RouteValidator : IPipelineHandler
    {
        public IList<MethodAndRoute> MethodsAndRoutes { get; set; }

        public ExitRule ExitRule { get; set; } = ExitRule.ExitIfNoMatch;        

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpRequestMessage httpRequestMessage))
            {
                //var match = MethodsAndRoutes.FirstOrDefault(x => )
            }

            await next.Invoke();
        }
    }
}
