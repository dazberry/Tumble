using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace Tumble.Handlers.Security
{
    public class MethodLimitHandler : IPipelineHandler<HttpRequestMessage, IContextResolver<HttpResponseMessage>>
    {
        public HttpMethod[] RefusedMethods { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage, IContextResolver<HttpResponseMessage> httpResponseMessage)
        {            
            if (RefusedMethods.Any(x => x == httpRequestMessage.Method))
            {
                var response = HttpResponseBuilder
                    .HttpResponseMessage()
                    .WithReasonPhrase("Method refused")
                    .WithStatusCode(403);

                httpResponseMessage.Set(response.Build());
            }
            else
                await next.Invoke();
        }
    }
}
