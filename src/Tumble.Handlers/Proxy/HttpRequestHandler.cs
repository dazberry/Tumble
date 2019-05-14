using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Contexts;
using Tumble.Handlers.Proxy.Contexts;
using Tumble.Handlers.Proxy.Converters;

namespace Tumble.Handlers.Proxy
{
    /// <summary>
    /// Requires IHttpContextAccessor, IHttpRequestMessageContext. Converts HttpContext to HttpRequestMessage
    /// </summary>
    public class HttpRequestHandler : IPipelineHandler<IHttpContextAccessor, IHttpRequestMessageContext>
    {
        public string[] HeadersToRemove = new[] { "Host" };

        public async Task InvokeAsync(PipelineDelegate next, IHttpContextAccessor httpContextContext, IHttpRequestMessageContext httpRequestMessageContext)
        {            
            PipelineConverters.Convert(
                    httpContextContext.HttpContext.Request, 
                out HttpRequestMessage httpRequestMessage, HeadersToRemove);

            httpRequestMessageContext.HttpRequestMessage = httpRequestMessage;            

            await next.Invoke();
        }
    }
}
