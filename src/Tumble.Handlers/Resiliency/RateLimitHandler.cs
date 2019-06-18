using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Resiliency
{
    public enum RateLimiterEnum { RateLimited };

    public class RateLimitHandler : IPipelineHandler<HttpRequestMessage>
    {
        public int MaxClients { get; set; }
        private int _activeClients = 0;

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage)
        {
            var count = Interlocked.Increment(ref _activeClients);
            try
            {
                if (count <= MaxClients)
                {
                    //context.Remove(RateLimiterEnum.RateLimited);
                    await next.Invoke();
                    return;
                }
                //context.Add(RateLimiterEnum.RateLimited);
            }
            finally
            {
                Interlocked.Decrement(ref _activeClients);
            }
        }
    }
}
