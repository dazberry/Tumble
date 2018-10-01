using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Handlers.Rtpi;
using PipelinedApi.Models;
using Tumble.Core;
using PipelinedApi.Handlers;

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
                .AddHandler<SetEndpoint>((handler, ctx) =>
                {
                    ctx.Add("endpoint", "/realtimebusinformation");
                })
                .AddHandler<SetStopId>((handler, ctx) =>
                {
                    handler.MakeOptional();
                    ctx.Add("stopId", stopId);
                })
                .AddHandler<SetOperatorId>((handler, ctx) =>
                {
                    handler.MakeOptional();
                    ctx.Add("operatorId", operatorId);
                })
                .AddHandler<SetRouteId>((handler, ctx) =>
                {
                    handler.MakeOptional();
                    ctx.Add("routeId", routeId);
                })
                .AddHandler<SetMaxResults>((handler, ctx) =>
                {
                    handler.MakeOptional();
                    ctx.Add("maxResults", maxResults.ToString(), maxResults.HasValue);
                })
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseSuccessResponse<ArrivalInfo>>()
                .InvokeAsync();
                 
                
            return Ok(context.Get<ApiResponse<ArrivalInfo>>("response"));                         
        }

        [HttpGet("{stopId}/information")]
        public async Task<IActionResult> GetStopInformation([FromRoute]string stopId)
        {
            var context = await new PipelineRequestBuilder(_handlers)
                            .AddHandler<SetEndpoint>((handler, ctx) =>
                                ctx.Add("endpoint", "/busstopinformation"))                                
                            .AddHandler<SetStopId>((handler, ctx) =>
                                ctx.Add("stopId", stopId))
                            .AddHandler<InvokeGetRequest>()
                            .AddHandler<ParseSuccessResponse<StopInfo>>()
                            .InvokeAsync();
                            
            var result = context.Get<ApiResponse<StopInfo>>("response");
            return Ok(result);
        }
    }
}
