using System.Threading;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Resiliency
{

    public enum RateLimiterEnum { RateLimited };

    public class RateLimiter : IPipelineHandler
    {        
        public int MaxClients { get; set; }
        private int _activeClients = 0;
        
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {           
            var count = Interlocked.Increment(ref _activeClients);
            try
            {
                if (count <= MaxClients)
                {
                    context.Remove(RateLimiterEnum.RateLimited);
                    await next.Invoke();
                    return;
                }
                context.Add(RateLimiterEnum.RateLimited);               
            }
            finally
            {
                Interlocked.Decrement(ref _activeClients);
            }
        }
    }    
}
