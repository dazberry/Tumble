using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Handlers
{
    public class GenerateObjectResult<T> : IPipelineHandler
    {
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            try
            {
                await next.Invoke();
            }
            finally
            {

                IActionResult objectResult = null;
                if (context.Get("response", out T response))
                    objectResult = new ObjectResult(response) { StatusCode = 200 };
                else
                {
                    objectResult = new ObjectResult(
                        new
                        {
                            context.Id,
                            notifications = context
                                .Get<Notification>()
                                .Select((x, i) =>
                                new
                                {
                                    id = i + 1,
                                    Handler = x.Handler.ToString(),
                                    x.ErrorMessage
                                })
                        })
                    { StatusCode = 500 };
                }

                context.Add<IActionResult>(objectResult);
            }
        }
    }
}
