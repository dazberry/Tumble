using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Client.Extensions;
using System.Net.Http;

namespace Tumble.Handlers.Resiliency
{
    public enum FixedIntervalRetryEnum { RetryCountExceeded }

    public class FixedIntervalRetryHandler : IPipelineHandler<HttpResponseMessage>
    {
        public FixedIntervalRetryConfiguration Configuration { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, HttpResponseMessage httpResponseMessage)
        {
            //context.Remove(FixedIntervalRetryEnum.RetryCountExceeded);           
            int index = 0;
            await next.Invoke();
           
            while (httpResponseMessage.IsTransientFailure())
            {
                var delay = Configuration.GetInterval(index);
                if (!delay.HasValue)
                {
                    //context.Add(FixedIntervalRetryEnum.RetryCountExceeded);
                    break;
                }

                await Task.Delay(delay.Value);
                await next.Invoke();
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
}
