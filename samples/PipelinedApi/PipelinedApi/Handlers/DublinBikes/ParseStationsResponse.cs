using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelinedApi.Models;
using Tumble.Core;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class ParseStationsResponse : IPipelineHandler
    {        
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpResponseMessage responseMessage))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<IEnumerable<DublinBikeStation>>
                        (result, new UnixDateTimeConverter());
                    context.Add("response", response);                   
                }
            }
            else
                throw new PipelineDependencyException<HttpResponseMessage>(this);
        }
    }

    
}
