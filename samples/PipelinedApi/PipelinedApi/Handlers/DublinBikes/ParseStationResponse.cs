﻿using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PipelinedApi.Models;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class ParseStationResponse : IPipelineHandler<HttpResponseMessage, IContextResolver<DublinBikeStation>>
    {        
        public async Task InvokeAsync(PipelineDelegate next, HttpResponseMessage httpResponseMessage, IContextResolver<DublinBikeStation> dublinBikeStation)
        {            

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<DublinBikeStation>(result);
                dublinBikeStation.Set(response);                
            }

            await next.Invoke();            
        }
    }

    
}
