using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Handlers;
using PipelinedApi.Handlers.Rtpi;
using PipelinedApi.Models;
using Tumble.Client.Handlers;
using Tumble.Core;

namespace PipelinedApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Route")]
    public class RouteController : Controller
    {
        private readonly PipelineHandlerCollection _handlers;

        public RouteController(PipelineHandlerCollection pipelineHandlerCollection)
        {
            _handlers = pipelineHandlerCollection;
        }


        [HttpGet("{operatorId}")]
        public async Task<IActionResult> GetRouteInformation([FromRoute] string operatorId)
        {
            var pipeline = new PipelineRequest()
                .AddHandlerFromCollection<SetEndpoint>(_handlers)
                .AddHandler<ContextQueryParameters>(handler =>
                    handler.Add("operator"))
                .AddHandlerFromCollection<InvokeGetRequest>(_handlers)
                .AddHandler<ParseSuccessResponse<OperatorAndRoute>>();                

            var context = new PipelineContext()
                .Add("endpoint", "/routeListInformation")
                .Add("operator", operatorId);

            await pipeline.InvokeAsync(context);

            return Ok(context.Get<ApiResponse<OperatorAndRoute>>("response"));
        }


        [HttpGet("{operatorId}/{routeId}")]
        public async Task<IActionResult> GetRouteInformation([FromRoute] string operatorId, [FromRoute] string routeId)
        {
            var pipeline = new PipelineRequest()
                .AddHandlerFromCollection<SetEndpoint>(_handlers)
                .AddHandler<ContextQueryParameters>(handler =>
                    handler.Add("operator")
                           .Add("routeId"))
                .AddHandlerFromCollection<InvokeGetRequest>(_handlers)
                .AddHandler<ParseSuccessResponse<RouteInfo>>();

            var context = new PipelineContext()
                .Add("endpoint", "/routeInformation")
                .Add("operator", operatorId)
                .Add("routeId", routeId);

            await pipeline.InvokeAsync(context);

            return Ok(context.Get<ApiResponse<RouteInfo>>("response"));
        }
    }
}