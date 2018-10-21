using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Handlers.Rtpi;
using PipelinedApi.Models;
using Tumble.Core;
using PipelinedApi.Handlers;
using Tumble.Client.Handlers;
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

        private PipelineRequest GetRealtimeBusInfoPipeline() =>
            new PipelineRequest()            
                .AddHandler<ContextParameters>(
                    handler => handler
                        .Add("endpoint", "/realtimebusinformation"))
                .AddHandler(_handlers.Get<SetEndpoint>())
                .AddHandler<ContextQueryParameters>(
                    handler => handler
                        .Add("stopId")
                        .Add("operator", true)
                        .Add("routeId", true)
                        .Add("maxResults", true))                
                .AddHandler(_handlers.Get<InvokeGetRequest>())
                .AddHandler<ParseSuccessResponse<ArrivalInfo>>();
                
        [HttpGet("{stopId}/arrival")]
        public async Task<IActionResult> GetByStopNumber([FromRoute]string stopId, [FromQuery]string routeId, [FromQuery]string operatorId, [FromQuery]int? maxResults)
        {           
            var context = await new PipelineRequest()
                    .AddHandler<GenerateObjectResult<ApiResponse<ArrivalInfo>>>()
                    .AddHandlers(GetRealtimeBusInfoPipeline())
                    .InvokeAsync(ctx => 
                        ctx.Add("stopId", stopId)
                        .Add("operator", operatorId, true)
                        .Add("routeId", routeId, true)
                        .Add("maxResults", maxResults.ToString(), true));           

            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);
        }

        private PipelineRequest GetBusStopInformationPipeline() =>
            new PipelineRequest()
                .AddHandler<ContextParameters>(
                    handler => handler
                        .Add("endpoint", "/busstopinformation"))
                .AddHandler(_handlers.Get<SetEndpoint>())
                .AddHandler<ContextQueryParameters>(
                    handler => handler
                        .Add("stopId"))
                .AddHandler(_handlers.Get<InvokeGetRequest>())
                .AddHandler<ParseSuccessResponse<StopInfo>>();

        [HttpGet("{stopId}/information")]
        public async Task<IActionResult> GetStopInformation([FromRoute]string stopId)
        {
            var context = await new PipelineRequest()
                    .AddHandler<GenerateObjectResult<ApiResponse<StopInfo>>>()
                    .AddHandlers(GetBusStopInformationPipeline())
                    .InvokeAsync(ctx =>
                        ctx.Add("stopId", stopId));
           
            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);
        }
    }
}
