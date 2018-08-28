using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelinedApi.Models;
using PipelinedApi.Routes.Common;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Pipelines
{
    public static class ByStopNumberPipeline
    {

        public class ValidateParams : IPipelineHandler
        {
            public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
            {
                var result = context.Get("stopId", out int stopId);
                if (result)                
                    result = stopId > 0 && stopId < 9999;
                if (result)
                    await next.Invoke();
                else
                    context.AddNotification(this, "Invalid or missing stopId");                
            }
        }

        public class BuildUri : IPipelineHandler
        {
            public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
            {
                context.Get("stopId", out int stopId);
                if (context.Get("endpoint", out string endpoint))
                {
                    var builder = new UriBuilder(endpoint);
                    builder.Query = new QueryBuilder()
                    {
                        { "stopId", stopId.ToString() },
                        { "format", "json" }
                    }
                    .ToString();

                    context.Add(builder.Uri);
                    await next.Invoke();
                }
                else
                    context.AddNotification(this, "missing endpoint information");

            }
        }        

        public class InvokeGetRequest : IPipelineHandler
        {
            public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
            {
                if (context.GetFirst(out Uri uri))
                {
                    var response = await HttpClientRequest
                        .Get()
                        .Uri(uri)
                        .InvokeAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        context.Add(response);
                        await next.Invoke();
                    }
                    else
                        context.Add(new Notification(this, "error message"));
                }
                else
                    context.AddNotification(this, "Not URI specified");
            }
        }

        public class ParseResponse : IPipelineHandler
        {
            public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
            {
                if (context.GetFirst(out HttpResponseMessage responseMessage))
                {                    
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<ApiResponse<ArrivalInfo>>(result, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" });
                    context.Add("response", response);
                    //context.Add("response", result);
                }                
            }
        }
        
        

        public static PipelineRequest Get()
        {
            return new PipelineRequest()
                .AddHandler<ValidateNamedParam<int>>(act =>
                {
                    act.ParamName = "stopId";
                    act.ValidateAction = (value) => value > 1000 && value < 9999;
                })
                .AddHandler<BuildUri>()
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseResponse>();
        }
    }
}
