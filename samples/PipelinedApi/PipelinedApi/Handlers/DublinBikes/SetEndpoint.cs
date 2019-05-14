using System;
using System.Threading.Tasks;
using Tumble.Client.Contexts;
using Tumble.Core;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class SetEndpoint : IPipelineHandler<IEndpointContext, IUriContext>
    {
        public Uri BaseUrl { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IEndpointContext endpointContext, IUriContext uriContext)
        {
            if (BaseUrl == null)
            {
                //context.AddNotification(this, "Missing BaseUrl value");
                return;
            }
            var uri = BaseUrl.Append(endpointContext.EndPoint);
            uriContext.Uri = uri;

            await next.Invoke();
        }
    }
}
