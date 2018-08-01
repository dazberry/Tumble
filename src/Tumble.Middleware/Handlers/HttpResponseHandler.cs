using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tumble.Converters;
using Tumble.Core;

namespace Tumble.Handlers
{
    public class HttpResponseHandler : IPipelineHandler
    {
        public string[] HeadersToRemove = new[] { "transfer-encoding", "X-Powered-By" };

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            await next.Invoke();

            if (!context.GetFirst(out HttpContext httpContext))
                throw new PipelineDependencyException<HttpContext>(this);

            if (!context.GetFirst(out HttpResponseMessage httpResponseMessage))
                throw new PipelineDependencyException<HttpResponseMessage>(this);

            await PipelineConverters.Convert(httpResponseMessage, httpContext.Response, HeadersToRemove);            
        }
    }
}
