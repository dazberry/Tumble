using System;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Handlers;

namespace PipelinedApi.Handlers.Luas
{
    public interface IEndpointContext
    {
        string Endpoint { get; set; }
    }

    public class SetEndpoint : IPipelineHandler<IEndpointContext>
    {
        public Uri BaseUrl { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IEndpointContext context)
        {
            context.Endpoint = BaseUrl;
            await next.Invoke();
        }
    }
}
