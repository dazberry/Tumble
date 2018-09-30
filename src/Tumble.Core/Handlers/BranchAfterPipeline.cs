using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core.Handlers
{
    public class BranchAfterPipeline : IPipelineHandler
    {        
        public PipelineRequest AdditionalBranch { get; set; }        
        public Func<PipelineContext, bool> BeforeAdditionalBranchAction { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {                        
            await next.Invoke();
            if (BeforeAdditionalBranchAction?.Invoke(context) ?? true)
                await AdditionalBranch?.InvokeAsync(context);
        }
    }
}
