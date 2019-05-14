using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Handlers
{   
    public interface IParseResponseToStringContext
    {
        string Response { get; set; }
    }

    public class ParseResponseToStringHandler : IPipelineHandler<IHttpResponseMessageContext, IParseResponseToStringContext>
    {
        public async Task InvokeAsync(PipelineDelegate next, IHttpResponseMessageContext context, IParseResponseToStringContext context2)
        {
            await next.Invoke();

            var resp = context.HttpResponseMessage;
            if (resp != null)            
            {
                if (resp.IsSuccessStatusCode)
                {
                    var response = await resp.Content.ReadAsStringAsync();
                    context2.Response = response;                    
                }
            }
            //else
            //    throw new PipelineDependencyException<HttpResponseMessage>(this);
        }

    }
}
