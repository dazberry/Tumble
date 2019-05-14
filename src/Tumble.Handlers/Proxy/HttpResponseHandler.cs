using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Contexts;
using Tumble.Handlers.Proxy.Contexts;
using Tumble.Handlers.Proxy.Converters;

namespace Tumble.Handlers.Proxy
{
    /// <summary>
    /// Requires IHttpContextAccessor, IHttpResponseMessageContext. Sets HttpContext.Response from HttpResponseMessage
    /// </summary>
    public class HttpResponseHandler : IPipelineHandler<IHttpContextAccessor, IHttpResponseMessageContext>
    {
        public string[] HeadersToRemove = new[] { "transfer-encoding", "X-Powered-By" };

        public async Task InvokeAsync(PipelineDelegate next, IHttpContextAccessor httpContextContext, IHttpResponseMessageContext httpResponseMessageContext)
        {
            await next.Invoke();

            await PipelineConverters.Convert(
                httpResponseMessageContext.HttpResponseMessage,
                httpContextContext.HttpContext.Response,
                HeadersToRemove); 
        }
    }
}
