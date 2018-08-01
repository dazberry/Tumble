using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tumble.Converters;
using Tumble.Core;

namespace Tumble.Handlers
{
    public class HttpRequestHandler : IPipelineHandler
    {
        public string[] HeadersToRemove = new[] { "Host" };

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (!context.GetFirst(out HttpContext httpContext))
                throw new PipelineDependencyException<HttpContext>(this);
                     
            PipelineConverters.Convert(httpContext.Request, out HttpRequestMessage httpRequestMessage, HeadersToRemove);
            context
                .Add(httpRequestMessage)
                .Add(httpContext.RequestAborted);            

            await next.Invoke();
        }
    }
}
