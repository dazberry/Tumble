using System;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Handlers
{
    public class InvokeGetRequest : IPipelineHandler
    {       
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out Uri uri))
            {
                var response = await HttpClientRequest
                    .Get()
                    .Uri(uri)
                    .InvokeAsync();

                if (response.IsSuccessStatusCode)
                {
                    context.Add(response);
                    await next.Invoke();
                };                
            }
            else
                context.AddNotification(this, "No URI specified");
        }
    }
}
