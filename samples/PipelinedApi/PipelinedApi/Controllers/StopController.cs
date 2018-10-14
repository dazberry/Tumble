using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Handlers.Rtpi;
using PipelinedApi.Models;
using Tumble.Core;
using PipelinedApi.Handlers;
using Tumble.Client.Handlers;

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
            var pipeline = new PipelineRequest()
                .AddHandlerFromCollection<SetEndpoint>(_handlers)
                .AddHandler<ContextQueryParameters>(
                    handler => handler
                        .Add("stopId")
                        .Add("operator", true)
                        .Add("routeId", true)
                        .Add("maxResults", true))
                .AddHandlerFromCollection<InvokeGetRequest>(_handlers)
                .AddHandler<ParseSuccessResponse<ArrivalInfo>>();

            var context = new PipelineContext()
                    .Add("endpoint", "/realtimebusinformation")
                    .Add("stopId", stopId)
                    .Add("operator", operatorId, true)
                    .Add("routeId", routeId, true)
                    .Add("maxResults", maxResults.ToString(), true);

            await pipeline.InvokeAsync(context);
                
            return Ok(context.Get<ApiResponse<ArrivalInfo>>("response"));                         
        }

        [HttpGet("{stopId}/information")]
        public async Task<IActionResult> GetStopInformation([FromRoute]string stopId)
        {
            var pipeline = new PipelineRequest()
                .AddHandlerFromCollection<SetEndpoint>(_handlers)
                .AddHandler<ContextQueryParameters>(
                    handler => handler
                        .Add("stopId"))
                .AddHandlerFromCollection<InvokeGetRequest>(_handlers)
                .AddHandler<ParseSuccessResponse<StopInfo>>();

            var context = new PipelineContext()
                    .Add("endpoint", "/busstopinformation")
                    .Add("stopId", stopId);

            await pipeline.InvokeAsync(context);
                            
            var result = context.Get<ApiResponse<StopInfo>>("response");
            return Ok(result);
        }
    }
}
