using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Contexts;
using Tumble.Handlers.Proxy.Converters;

namespace Tumble.Handlers.Proxy
{
    /// <summary>
    /// Requires HttpContext, IHttpResponseMessageContext. Sets HttpContext.Response from HttpResponseMessage
    /// </summary>
    public class HttpResponseHandler : IPipelineHandler<HttpResponseMessage, HttpContext>
    {
        public string[] HeadersToRemove = new[] { "transfer-encoding", "X-Powered-By" };

        public async Task InvokeAsync(PipelineDelegate next, HttpResponseMessage httpResponseMessage, HttpContext httpContext)
        {
            await next.Invoke();

            await PipelineConverters.Convert(   
                httpResponseMessage,                
                httpContext.Response,
                HeadersToRemove); 
        }
    }
}
