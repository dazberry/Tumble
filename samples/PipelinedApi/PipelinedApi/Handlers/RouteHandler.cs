using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Client.Extensions;

namespace PipelinedApi.Handlers
{
    public class RouteHandler : IPipelineHandler<IContextResolver<Uri>>
    {
        public string Route { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IContextResolver<Uri> context)
        {
            if (!string.IsNullOrEmpty(Route))            
                context.Set(context.Get().Append(Route));            

            await next.Invoke();
        }
    }
}
