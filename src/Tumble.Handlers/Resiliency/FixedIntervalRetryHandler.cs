using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Proxy.Contexts;
using Tumble.Client.Extensions;

namespace Tumble.Handlers.Resiliency
{
    public enum FixedIntervalRetryEnum { RetryCountExceeded }

    public class FixedIntervalRetryHandler : IPipelineHandler<IHttpRequestResponseContext>
    {
        public FixedIntervalRetryConfiguration Configuration { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestResponseContext context)
        {
            //context.Remove(FixedIntervalRetryEnum.RetryCountExceeded);           
            int index = 0;
            await next.Invoke();

            var resp = context.HttpResponseMessage;

            while (resp.IsTransientFailure())
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
