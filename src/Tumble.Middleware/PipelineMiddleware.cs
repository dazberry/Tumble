using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tumble.Core;

namespace Tumble.Middleware
{
    public enum PipelineMiddlewareEnum { Continue, Exit };
    
    public class PipelineMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PipelineRequest _pipelineRequest;
        private readonly PipelineMiddlewareConfiguration _middlewareConfiguration;

        public PipelineMiddleware(RequestDelegate next, PipelineRequest pipelineRequest) : this(next, pipelineRequest, null) { }

        public PipelineMiddleware(RequestDelegate next, PipelineRequest pipelineRequest, PipelineMiddlewareConfiguration middlewareConfiguration)
        {
            _next = next;            
            _pipelineRequest = pipelineRequest;
            _middlewareConfiguration = middlewareConfiguration ?? new PipelineMiddlewareConfiguration();
        }
        
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(_middlewareConfiguration.StartsWithSegment))
            {                             
                var pipelineContext = new PipelineContext();
                pipelineContext
                    .Add(httpContext);
                
                await _pipelineRequest.InvokeAsync(pipelineContext);

                if (pipelineContext.GetFirst(out PipelineMiddlewareEnum pipelineMiddlewareEnum))                    
                {
                    if (pipelineMiddlewareEnum == PipelineMiddlewareEnum.Exit)
                        return;
                }
                else
                if (_middlewareConfiguration.AfterPipelineInvoke == PipelineMiddlewareEnum.Exit)
                    return;
                                                                        
            }
            await _next(httpContext);            
        }
    }
}
