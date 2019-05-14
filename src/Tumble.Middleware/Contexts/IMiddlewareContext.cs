using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tumble.Middleware.Contexts
{
    public interface IMiddlewareContext : IHttpContextAccessor
    {        
        PipelineMiddlewareAfterInvoke MiddlewareCompletion { get; set; }
    };
}
