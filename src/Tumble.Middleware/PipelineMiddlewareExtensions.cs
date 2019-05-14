using Microsoft.AspNetCore.Builder;
using Tumble.Core;
using System;
using System.Linq;
using Tumble.Middleware.Contexts;

namespace Tumble.Middleware
{
    public static class ProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseProxyMiddleware<TMiddlewareContext>(
            this IApplicationBuilder builder,
            PipelineRequest pipelineRequest,
            PipelineMiddlewareConfiguration configuration = null)
            where
                TMiddlewareContext : IMiddlewareContext,
                new()
        {
            var args = new object[] { pipelineRequest, configuration }.Where(x => x != null).ToArray();
            return builder.UseMiddleware<PipelineMiddleware<TMiddlewareContext>>(args);
        }

        public static IApplicationBuilder UseProxyMiddleware<TMiddlewareContext>(
            this IApplicationBuilder builder,
            Action<PipelineRequest> pipelineRequestAction,
            PipelineMiddlewareConfiguration configuration = null)
            where
                TMiddlewareContext : IMiddlewareContext,
                new()
        {
            PipelineRequest request = new PipelineRequest();
            pipelineRequestAction?.Invoke(request);
            return UseProxyMiddleware<TMiddlewareContext>(builder, request, configuration);
        }

        public static IApplicationBuilder UseProxyMiddleware(
            this IApplicationBuilder builder,
            Action<PipelineRequest> pipelineRequestAction,
            PipelineMiddlewareConfiguration configuration = null)
        {
            PipelineRequest request = new PipelineRequest();
            pipelineRequestAction?.Invoke(request);
            return UseProxyMiddleware<MiddlewareContext>(builder, request, configuration);
        }

        public static IApplicationBuilder UseProxyMiddleware(
            this IApplicationBuilder builder,
            PipelineRequest pipelineRequest,
            PipelineMiddlewareConfiguration configuration = null)        
        {            
            var args = new object[] { pipelineRequest, configuration }.Where(x => x != null).ToArray();
            return builder.UseMiddleware<MiddlewareContext>(args);
        }


    }
}
