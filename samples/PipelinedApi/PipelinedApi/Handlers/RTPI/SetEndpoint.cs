using System;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Notifications;
using Tumble.Client.Extensions;

namespace PipelinedApi.Handlers.Rtpi
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
