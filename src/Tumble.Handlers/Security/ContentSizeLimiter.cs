using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;

namespace Tumble.Handlers.Security
{
    public class ContentSizeLimiter : IPipelineHandler
    {
        public long MaxRequestSize { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpRequestMessage httpRequestMessage))
            {                                
                if (httpRequestMessage.Content?.Headers.ContentLength > MaxRequestSize)
                {
                    var response = HttpResponseBuilder
                        .HttpResponseMessage()
                        .WithReasonPhrase("Content too large")
                        .WithStatusCode(500);

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
