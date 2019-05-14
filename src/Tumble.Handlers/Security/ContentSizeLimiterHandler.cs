using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Security
{
    public class ContentSizeLimiterHandler : IPipelineHandler<IHttpRequestResponseContext>
    {
        public long MaxRequestSize { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestResponseContext context)
        {
            var req = context.HttpRequestMessage;            
            if (req?.Content?.Headers.ContentLength > MaxRequestSize)
            {
                var response = HttpResponseBuilder
                    .HttpResponseMessage()
                    .WithReasonPhrase("Content too large")
                    .WithStatusCode(500);

                context.HttpResponseMessage = response.Build();
            }
            else
                await next.Invoke();
        }
    }
}
