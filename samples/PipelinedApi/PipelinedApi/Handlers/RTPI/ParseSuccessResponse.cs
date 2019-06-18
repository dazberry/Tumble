using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelinedApi.Models;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace PipelinedApi.Handlers.Rtpi
{
    public class ParseSuccessResponse<T> : IPipelineHandler<HttpResponseMessage, IContextResolver<T>>
    {
        public async Task InvokeAsync(PipelineDelegate next, HttpResponseMessage httpResponseMessage, IContextResolver<T> apiResponse)
        {        
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<T>
                    (result, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" });
                apiResponse.Set(response);
            }

            await next.Invoke();
        }

    }
}
