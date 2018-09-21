using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Models;
using PipelinedApi.Routes.Common;
using Tumble.Core;
using Tumble.Handlers.Miscellaneous;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PipelinedApi.Controllers
{
    [Route("api/[controller]")]
    public class StopController : Controller
    {
        private readonly PipelineHandlerCollection _handlers;

        public StopController(PipelineHandlerCollection pipelineHandlerCollection)
        {
            _handlers = pipelineHandlerCollection;            
        }

        [HttpGet("{stopId}/arrival")]
        public async Task<IActionResult> GetByStopNumber([FromRoute]string stopId, [FromQuery]string routeId, [FromQuery]string operatorId, [FromQuery]int? maxResults)
        {
            var context = await new PipelineRequestBuilder(_handlers)                                                
                .AddHandler<SetEndpoint>()
                .AddHandler<SetStopId>()
                .AddHandler<SetOperatorId>(
                    handler => handler.OptionalParameter = true)
                .AddHandler<SetRouteId>(
                    handler => handler.OptionalParameter = true)
                .AddHandler<SetMaxResults>(
                    handler => handler.OptionalParameter = true)
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseSuccessResponse<ArrivalInfo>>()
                .PipelineRequest
                .InvokeAsync(ctx =>
                    ctx.Add("stopId", stopId)
                       .Add("routeId", routeId)
                       .Add("operatorId", operatorId)
                       .Add("maxResults", maxResults.ToString(), maxResults.HasValue)
                       .Add("endpoint", "/realtimebusinformation")
                );

            return Ok(context.Get<ApiResponse<ArrivalInfo>>("response"));                         
        }

        [HttpGet("{stopId}/information")]
        public async Task<IActionResult> GetStopInformation([FromRoute]string stopId)
        {
            var context = await new PipelineRequestBuilder(_handlers)
                            .AddHandler<SetEndpoint>()
                            .AddHandler<SetStopId>()
                            .AddHandler<InvokeGetRequest>()
                            .AddHandler<ParseSuccessResponse<StopInfo>>()
                            .PipelineRequest
                            .InvokeAsync(ctx =>
                                ctx.Add("stopId", stopId)
                                   .Add("endpoint", "/busstopinformation")
                            );
            var result = context.Get<ApiResponse<StopInfo>>("response");
            return Ok(result);
        }
    }
}
