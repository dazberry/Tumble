using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Security
{
    public class MethodLimitHandler : IPipelineHandler<IHttpRequestResponseContext>
    {
        public HttpMethod[] RefusedMethods { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestResponseContext context)
        {
            var req = context.HttpRequestMessage;            
            if (RefusedMethods.Any(x => x == req.Method))
            {
                var response = HttpResponseBuilder
                    .HttpResponseMessage()
                    .WithReasonPhrase("Method refused")
                    .WithStatusCode(403);

                context.HttpResponseMessage = response.Build();
            }
            else
                await next.Invoke();
        }
    }
}
