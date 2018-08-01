using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client.Extensions;
using Tumble.Core;

namespace Tumble.Handlers.Resiliency
{
    public enum FixedIntervalRetryEnum { RetryCountExceeded }

    public class FixedIntervalRetryHandler : IPipelineHandler
    {
        public FixedIntervalRetryConfiguration Configuration { get; set; }
       
        protected virtual bool IsTransientFailure(PipelineContext context)
        {
            if (context.GetFirst(out HttpResponseMessage httpResponseMessage))            
                return httpResponseMessage.IsTransientFailure();
            return false;
        }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            context.Remove(FixedIntervalRetryEnum.RetryCountExceeded);

            int index = 0;            
            await next.Invoke();           
            while (IsTransientFailure(context))
            {
                var delay = Configuration.GetInterval(index);
                if (!delay.HasValue)
                {
                    context.Add(FixedIntervalRetryEnum.RetryCountExceeded);
                    break;
                }

                await Task.Delay(delay.Value);
                await next.Invoke();                    
            }

        }        
    }

    public class FixedIntervalRetryConfiguration
    {
        public int[] FixedIntervals { get; set; }

        public int? GetInterval(int index) =>
            index >= 0 && index < FixedIntervals.Length
                ? FixedIntervals[index]
                : (int?)null;
    }
}
