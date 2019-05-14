using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PipelinedApi.Models;
using Tumble.Core;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class ParseStationsResponse : IPipelineHandler
    {        
        public async Task InvokeAsync(IPipelineContext context, PipelineDelegate next)
        {
            if (context.Get(out HttpResponseMessage responseMessage))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<IEnumerable<DublinBikeStation>>(result);                    
                    context.Set(response);

                    await next.Invoke();
                }
            }
            //else
            //    throw new PipelineDependencyException<HttpResponseMessage>(this);
        }
    }

    
}
