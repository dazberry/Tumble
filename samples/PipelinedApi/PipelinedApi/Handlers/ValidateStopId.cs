using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Handlers
{
    public class ValidateStopId : IPipelineHandler
    {
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            var result = context.Get("stopId", out int stopId);
            if (result)
                result = stopId > 0 && stopId < 9999;
            if (result)
                await next.Invoke();
            else
                context.AddNotification(this, "Invalid or missing stopId");
        }
    }
}
