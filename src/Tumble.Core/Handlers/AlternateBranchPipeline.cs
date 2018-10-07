using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core.Handlers
{
    public class AlternateBranchPipeline : IPipelineHandler
    {
        public Func<PipelineContext, bool> UseAlternateBranchAction { get; set; }
        public PipelineRequest AlternateBranch { get; set; }
    
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            bool useAlternateBranch = UseAlternateBranchAction?.Invoke(context) ?? false;
            if (useAlternateBranch)
                await AlternateBranch.InvokeAsync(context);
            else
                await next.Invoke();
        }
    }
}
