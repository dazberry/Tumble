using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core.Handlers
{
    public class PipelineBranch : IPipelineHandler
    {
        public PipelineRequest Branch { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            await Branch?.InvokeAsync(context);
            await next.Invoke();
        }
    }
}
