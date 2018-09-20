using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Tumble.Core;
using Tumble.Core.Notifications;
using Tumble.Client.Extensions;

namespace PipelinedApi.Handlers
{
    public class SetEndpoint : IPipelineHandler
    {
        public Uri BaseUrl { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {            
            if (context.Get("endpoint", out string endPoint))              
            {
                var uri = BaseUrl.Append(endPoint);
                context.Add(uri);
                await next.Invoke();
            }
            else
                context.AddNotification(this, "missing endpoint information");

        }
    }
}
