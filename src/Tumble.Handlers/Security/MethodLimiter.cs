using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;

namespace Tumble.Handlers.Security
{
    public class MethodLimiter : IPipelineHandler
    {
        public HttpMethod[] RefusedMethods { get; set; }                

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpRequestMessage httpRequestMessage))
            {
                if (RefusedMethods.Any(x => x == httpRequestMessage.Method))
                { 
                    var response = HttpResponseBuilder
                        .HttpResponseMessage()
                        .WithReasonPhrase("Method refused")
                        .WithStatusCode(403);

                    context.Add(response);
                }
                else
                    await next.Invoke();
            }
            else
                throw new PipelineDependencyException<HttpRequestMessage>(this);
        }
    }
}
