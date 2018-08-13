using Microsoft.AspNetCore.Builder;
using Tumble.Core;
using System;
using System.Linq;

namespace Tumble.Middleware
{
    public static class ProxyMiddlewareExtensions
    {       
        public static IApplicationBuilder UseProxyMiddleware(this IApplicationBuilder builder, PipelineRequest pipelineRequest, PipelineMiddlewareConfiguration configuration = null)
        {
            
            var args = new object [] { pipelineRequest, configuration }.Where(x => x != null).ToArray();
            return builder.UseMiddleware<PipelineMiddleware>(args);
        }

        public static IApplicationBuilder UseProxyMiddleware(this IApplicationBuilder builder, Action<PipelineRequest> pipelineRequestAction, PipelineMiddlewareConfiguration configuration = null)
        {
            PipelineRequest request = new PipelineRequest();
            pipelineRequestAction?.Invoke(request);
            return UseProxyMiddleware(builder, request, configuration);
        }

    }
}
