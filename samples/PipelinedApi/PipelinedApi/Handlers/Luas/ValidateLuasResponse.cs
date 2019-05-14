using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;

namespace PipelinedApi.Handlers.Luas
{
    public class ValidateLuasResponse : IPipelineHandler
    {
        public async Task InvokeAsync(IPipelineContext context, PipelineDelegate next)
        {
            if (context.Get(out HttpResponseMessage responseMessage))
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
            //else
            //    throw new PipelineDependencyException<HttpRequestMessage>(this);
        }
    }
}
