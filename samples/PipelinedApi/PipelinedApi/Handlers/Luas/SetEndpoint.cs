using System;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Notifications;
using Tumble.Client.Extensions;

namespace PipelinedApi.Handlers.Luas
{
    public class SetEndpoint : IPipelineHandler
    {
        public Uri BaseUrl { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            context.Add(BaseUrl);
            await next.Invoke();
        }
    }
}
