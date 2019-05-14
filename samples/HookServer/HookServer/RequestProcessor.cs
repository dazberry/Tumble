using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;

namespace HookServer
{
    public class RequestProcessor : IPipelineHandler
    {
        public async Task InvokeAsync(IPipelineContext context, PipelineDelegate next)
        {


            await next.Invoke();            
        }
    }
}
