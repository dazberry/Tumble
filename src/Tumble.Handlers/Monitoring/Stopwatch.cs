using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers
{
    public class Stopwatch : IPipelineHandler
    {
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            try
            {
                stopWatch.Start();
                await next.Invoke();
            }
            finally
            {                
                context.Add(stopWatch.Elapsed);
                stopWatch.Stop();
            }
        }
    }
}
