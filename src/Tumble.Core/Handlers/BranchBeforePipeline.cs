using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core.Handlers
{
    public class BranchBeforePipeline : IPipelineHandler
    {        
        public PipelineRequest AdditionalBranch { get; set; }        
        public Func<PipelineContext, bool> AfterAdditionBranchAction { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            await AdditionalBranch?.InvokeAsync(context);
            if (AfterAdditionBranchAction?.Invoke(context) ?? true)
                await next.Invoke();
        }
    }
}
