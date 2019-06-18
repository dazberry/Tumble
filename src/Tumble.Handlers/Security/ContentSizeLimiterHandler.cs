using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace Tumble.Handlers.Security
{
    public class ContentSizeLimiterHandler : IPipelineHandler<HttpRequestMessage, IContextResolver<HttpResponseMessage>>
    {
        public long MaxRequestSize { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage, IContextResolver<HttpResponseMessage> httpResponseMessage)
        {            
            if (httpRequestMessage?.Content?.Headers.ContentLength > MaxRequestSize)
            {
                var response = HttpResponseBuilder
                    .HttpResponseMessage()
                    .WithReasonPhrase("Content too large")
                    .WithStatusCode(500);

                httpResponseMessage.Set(response.Build());
            }
            else
                await next.Invoke();
        }
    }
}
