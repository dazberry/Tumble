using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tumble.Client.Contexts;
using Tumble.Core;

namespace Tumble.Client.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientRequestHandler : IPipelineHandler<IHttpClientContext>
    {
        public TimeSpan RequestTimeout { get; set; } = new TimeSpan(0, 1, 0);

        public async Task InvokeAsync(PipelineDelegate next, IHttpClientContext context)
        {
            HttpClientRequest request = new HttpClientRequest(context.HttpRequestMessage);

            CancellationTokenSource cts = new CancellationTokenSource(RequestTimeout);
            var cancellationTokens = new[] { context.CancellationToken };
            var token = cancellationTokens.Any()
                    ? CancellationTokenSource.CreateLinkedTokenSource(
                            cancellationTokens.Concat(new[] { cts.Token }).ToArray()
                      ).Token
                    : cts.Token;
            
            var result = await request.InvokeAsync(token);
            context.HttpResponseMessage = result;
            await next.Invoke();            
        }
    }
}
