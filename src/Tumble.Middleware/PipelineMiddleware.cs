using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tumble.Core;
using Tumble.Middleware.Contexts;

namespace Tumble.Middleware
{
    public enum PipelineMiddlewareAfterInvoke { Continue = 0, Exit = 1 };

    public class MiddlewareContext : IMiddlewareContext
    {
        public HttpContext HttpContext { get; set; }
        public PipelineMiddlewareAfterInvoke MiddlewareCompletion { get; set; }
    }

    public class PipelineMiddleware<T>
        where T : IMiddlewareContext, new()
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
                var context = new T
                {
                    HttpContext = httpContext,
                    MiddlewareCompletion = PipelineMiddlewareAfterInvoke.Continue
                };

                await _pipelineRequest.InvokeAsync(context);

                if (_middlewareConfiguration.AfterPipelineInvoke == PipelineMiddlewareAfterInvoke.Exit)
                    return;
                if (context.MiddlewareCompletion == PipelineMiddlewareAfterInvoke.Exit)                    
                    return;                
                                                                                        
            }
            await _next(httpContext);            
        }
    }
}
