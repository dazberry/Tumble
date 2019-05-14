using System;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Client.Extensions;
using Tumble.Core.Handlers;

namespace PipelinedApi.Handlers.Rtpi
{
    public class EndpointContext : PipelineContext
    {
        public string Endpoint { get; set; }
    }

    public class SetEndpoint : PipelineHandler<EndpointContext>
    {
        public Uri BaseUrl { get; set; }

        public override async Task InvokeAsync(EndpointContext context, PipelineDelegate next)
        {            
            if (context.Get(out string endPoint))              
            {
                if (BaseUrl == null)
                {
                    context.AddNotification(this, "Missing BaseUrl value");
                    return;
                }
                var uri = BaseUrl.Append(endPoint);
                context.Set(uri);
                await next.Invoke();
            }
            else
                context.AddNotification(this, "Missing endpoint information");

        }

    }
}
