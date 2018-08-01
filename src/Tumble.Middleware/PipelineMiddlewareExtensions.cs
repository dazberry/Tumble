using Microsoft.AspNetCore.Builder;
using Tumble.Core;
using System;

namespace Tumble.Middleware
{
    public static class ProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseProxyMiddleware(this IApplicationBuilder builder, PipelineRequest pipelineRequest)
        {
            return builder.UseMiddleware<PipelineMiddleware>(pipelineRequest);
        }

        public static IApplicationBuilder UseProxyMiddleware(this IApplicationBuilder builder, Action<PipelineRequest> pipelineRequestAction)
        {
            PipelineRequest request = new PipelineRequest();
            pipelineRequestAction?.Invoke(request);
            return UseProxyMiddleware(builder, request);
        }

    }
}
