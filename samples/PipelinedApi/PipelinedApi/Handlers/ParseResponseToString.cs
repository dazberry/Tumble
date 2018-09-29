using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;

namespace PipelinedApi.Handlers
{
    public class ParseResponseToString : IPipelineHandler
    {
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpResponseMessage responseMessage))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await responseMessage.Content.ReadAsStringAsync();
                    context.Add("response", response);
                }
            }
            else
                throw new PipelineDependencyException<HttpResponseMessage>(this);
        }

    }
}
