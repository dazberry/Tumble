using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Handlers;
using Tumble.Core;
using Tumble.Handlers.Miscellaneous;

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
            var context = await new PipelineRequestBuilder(_handlers)
                .AddHandler<SetEndpoint>()
                .AddHandler<SetOperatorId>()
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseResponseToString>()
                .PipelineRequest
                .InvokeAsync(ctx =>
                    ctx.Add("operatorId", operatorId)                    
                    .Add("endpoint", "/routeListInformation"));

            return Ok(context.Get<string>("response"));

        }


        [HttpGet("{operatorId}/{routeId}")]
        public async Task<IActionResult> GetRouteInformation([FromRoute] string operatorId, [FromRoute] string routeId)
        {
            var context = await new PipelineRequestBuilder(_handlers)
                .AddHandler<SetEndpoint>()
                .AddHandler<SetOperatorAndRouteIds>()
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseResponseToString>()                
                .PipelineRequest
                .InvokeAsync(ctx =>
                    ctx.Add("operatorId", operatorId)
                    .Add("routeId", routeId)
                    .Add("endpoint", "/routeInformation"));

            return Ok(context.Get<string>("response"));

        }
    }
}