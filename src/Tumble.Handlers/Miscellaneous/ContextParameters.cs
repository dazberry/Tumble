using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Miscellaneous
{
    public class ContextParameters : PipelineContext, IPipelineHandler
    {        
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            context.AddFromContext(this);
            await next.Invoke();            
        }
    }
}
