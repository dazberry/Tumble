using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Contexts;
using PipelinedApi.Handlers;
using PipelinedApi.Handlers.Rtpi;
using PipelinedApi.Handlers.RTPI;
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

        private PipelineRequest GetRouteListInformationPipeline() =>
            new PipelineRequest()
                .AddHandler(_handlers.Get<SetRTPIEndpoint>())
                .AddHandler<QueryParametersHander>(ctx =>
                    ctx.Add("endpoint", "/routeListInformation")
                       .Add("operator"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseSuccessResponse<OperatorAndRoute>>();
                                                          

        [HttpGet("{operatorId}")]
        public async Task<IActionResult> GetRouteInformation([FromRoute] string operatorId) 
        {
            var context = await GetRouteListInformationPipeline()
                .AddHandler<GenerateObjectResult<OperatorAndRoute>>()
                .InvokeAsync(new RTPIContext<OperatorAndRoute>(),
                    ctx =>
                        ctx.Add("operator", operatorId));

            return context.ObjectResult;
        }

        //private PipelineRequest GetRouteInformationPipeline() =>
        //    new PipelineRequest()
        //        //.AddHandler<ContextParameters>(
        //        //    handler => handler
        //        //        .Add("endpoint", "/routeInformation"))
        //        .AddHandler(_handlers.Get<SetEndpointHandler>())
        //        .AddHandler<ContextQueryParameters>(handler =>
        //            handler.Add("operator")
        //                   .Add("routeId"))
        //        .AddHandler(_handlers.Get<InvokeGetRequest>())
        //        .AddHandler<ParseSuccessResponse<RouteInfo>>();

        [HttpGet("{operatorId}/{routeId}")]
        public async Task<IActionResult> GetRouteInformation([FromRoute] string operatorId, [FromRoute] string routeId)
        {
            //var context = await new PipelineRequest()
            //    .AddHandler<GenerateObjectResult<ApiResponse<RouteInfo>>>()
            //    .AddHandlers(GetRouteInformationPipeline())
            //    .InvokeAsync(ctx => ctx
            //        .Add("operator", operatorId)
            //        .Add("routeId", routeId));

            //if (context.GetFirst(out IActionResult response))
            //    return response;

            return new StatusCodeResult(500);
        }
    }
}