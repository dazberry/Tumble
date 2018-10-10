using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Client.Extensions;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class SetEndpoint : IPipelineHandler
    {
        public Uri BaseUrl { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.Get("endpoint", out string endPoint))
            {
                if (BaseUrl == null)
                {
                    context.AddNotification(this, "Missing BaseUrl value");
                    return;
                }
                var uri = BaseUrl.Append(endPoint);
                context.Add(uri);
                await next.Invoke();
            }
            else
                context.AddNotification(this, "Missing endpoint information");
        }
    }
}
