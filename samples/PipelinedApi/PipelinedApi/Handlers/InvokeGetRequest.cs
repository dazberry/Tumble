using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Handlers    
{
    /// <summary>    
    /// Issue HTTP GET
    /// <para></para>        
    /// Requires: Uri, IContextWriter&lt;HttpResponseMessage&gt;
    /// </summary>    
    public class InvokeGetRequest : IPipelineHandler<Uri, IHttpResponseMessageContext>
    {       
        public async Task InvokeAsync(PipelineDelegate next, Uri uri, IHttpResponseMessageContext responseMessage)
        {            
            var response = await HttpClientRequest
                .Get()
                .Uri(uri)
                .InvokeAsync();

            responseMessage.HttpResponseMessage = response;            
            await next.Invoke();            
        }
    }
}
