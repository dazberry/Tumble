using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Client.Handlers
{
    public class ClientRequest : IPipelineHandler
    {
        public TimeSpan RequestTimeout { get; set; } = new TimeSpan(0, 1, 0);

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (!context.GetFirst(out HttpRequestMessage httpRequestMessage))
                throw new PipelineDependencyException<HttpRequestMessage>(this);
           
            HttpClientRequest request = new HttpClientRequest(httpRequestMessage);
            CancellationTokenSource cts = new CancellationTokenSource(RequestTimeout);            

            var cancellationTokens = context.Get<CancellationToken>();
            var token = cancellationTokens.Any()
                    ? CancellationTokenSource.CreateLinkedTokenSource(
                        cancellationTokens.Concat(new[] { cts.Token }).ToArray()
                      ).Token
                    : cts.Token;
            
            try
            {
                var result = await request.InvokeAsync(token);
                context.Add(result);
                await next.Invoke();
            }
            catch (HttpRequestException ex)
            {
                throw new PipelineException(this, ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PipelineException(this, ex.Message, ex);
            }
        }
    }
}
