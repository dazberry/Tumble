﻿using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelinedApi.Models;
using Tumble.Core;

namespace PipelinedApi.Handlers.Rtpi
{
    public class ParseSuccessResponse<T> : IPipelineHandler
    {
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpResponseMessage responseMessage))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<ApiResponse<T>>
                        (result, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" });
                    context.Add("response", response);

                    await next.Invoke();
                }
            }
            else
                throw new PipelineDependencyException<HttpResponseMessage>(this);
        }

    }
}
