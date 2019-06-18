using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Models;
using Tumble.Core;
using Tumble.Core.Contexts;

namespace PipelinedApi.Handlers
{

    public interface IObjectResultContext
    {
        ObjectResult ObjectResult { get; set; }
    }

    public class GenerateObjectResult<TResponse> : IPipelineHandler<HttpResponseMessage, TResponse, IObjectResultContext>
    {

        private void SetObjectResult<T>(IObjectResultContext context, T value, int statusCode) =>        
            context.ObjectResult = new ObjectResult(value) { StatusCode = statusCode };        

        public async Task InvokeAsync(PipelineDelegate next, 
            HttpResponseMessage httpResponseMessage, 
            TResponse response, 
            IObjectResultContext ctx)
        {
            try
            {
                await next.Invoke();
            }
            finally
            {
                if (httpResponseMessage != null)
                {
                    if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                        SetObjectResult(ctx, 
                            response, 
                            200);                        
                    else                    
                        SetObjectResult(ctx,
                            await httpResponseMessage.Content.ReadAsStringAsync(),
                            (int)httpResponseMessage.StatusCode);                    
                }
                else                
                    SetObjectResult(ctx, 
                        "Internal server error", 
                        500);
            }
        }
    }
}
