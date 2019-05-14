using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Resiliency
{
    public enum RateLimiterEnum { RateLimited };

    public class RateLimitHandler : IPipelineHandler<IHttpRequestResponseContext>
    {
        public int MaxClients { get; set; }
        private int _activeClients = 0;

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestResponseContext context)
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
