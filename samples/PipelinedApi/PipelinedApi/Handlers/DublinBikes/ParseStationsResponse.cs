using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PipelinedApi.Models;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class ParseStationsResponse : IPipelineHandler<HttpResponseMessage, IContextResolver<IEnumerable<DublinBikeStation>>>
    {        
        public async Task InvokeAsync(PipelineDelegate next, HttpResponseMessage httpResponseMessage, IContextResolver<IEnumerable<DublinBikeStation>> dublinBikeStations)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<IEnumerable<DublinBikeStation>>(result);
                dublinBikeStations.Set(response);
            }

            await next.Invoke();
        }
    }

    
}
