using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Handlers
{ 
    /// <summary>    
    /// Sets Endpoint from BaseUrl
    /// <para></para>        
    /// Requires: IEndpointContext
    /// </summary>    
    public abstract class SetEndpointHandler : IPipelineHandler<IContextResolver<Uri>>
    {
        public virtual Uri BaseUrl { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IContextResolver<Uri> ctx)
        {
            ctx.Set(BaseUrl);            
            await next.Invoke();
        }
    }
}
