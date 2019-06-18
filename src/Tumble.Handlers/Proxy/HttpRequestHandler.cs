using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;
using Tumble.Handlers.Proxy.Converters;

namespace Tumble.Handlers.Proxy
{
    /// <summary>
    /// Requires HttpContext, IHttpRequestMessageContext. Creates a HttpRequestMessage from a HttpContext
    /// </summary>
    public class HttpRequestHandler : IPipelineHandler<HttpContext, IContextResolver<HttpRequestMessage>>
    {
        public string[] HeadersToRemove = new[] { "Host" };

        public async Task InvokeAsync(PipelineDelegate next, HttpContext httpContext, IContextResolver<HttpRequestMessage> httpRequestMessage)
        {            
            PipelineConverters.Convert(
                    httpContext.Request, 
                out HttpRequestMessage requestMessage, HeadersToRemove);

            httpRequestMessage.Set(requestMessage);            

            await next.Invoke();
        }
    }
}
