using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Handlers.Luas
{
    public class ValidateLuasResponse : IPipelineHandler
    {
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpResponseMessage responseMessage))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await responseMessage.Content.ReadAsStringAsync();
                    if (string.Compare("Exception:", response.Substring(0, 10), true) == 0)
                    {
                        context.AddNotification(this, response);
                        return;
                    }
                }
                else
                    context.AddNotification(this, $"Unsuccessful Response: {responseMessage.StatusCode}");

                await next.Invoke();
            }
            else
                throw new PipelineDependencyException<HttpRequestMessage>(this);
        }
    }
}
