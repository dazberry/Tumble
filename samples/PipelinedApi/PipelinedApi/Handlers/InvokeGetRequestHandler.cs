using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Handlers;

namespace PipelinedApi.Handlers    
{
   
    public interface IInvokeGetRequestContext 
    {
        Uri Uri { get; set; }
        HttpResponseMessage HttpResponseMessage { get; set; }
    }
    
    public class InvokeGetRequestHandler : IPipelineHandler<IInvokeGetRequestContext>
    {       
        public async Task InvokeAsync(PipelineDelegate next, IInvokeGetRequestContext context)
        {            
            var response = await HttpClientRequest
                .Get()
                .Uri(context.Uri)
                .InvokeAsync();

            if (response.IsSuccessStatusCode)
            {
                context.HttpResponseMessage = response;                    
                await next.Invoke();
            };
        }
    }
}
