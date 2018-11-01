using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Handlers.DublinBikes;
using PipelinedApi.Models;
using Tumble.Client.Handlers;
using Tumble.Core;

namespace PipelinedApi.Controllers
{
    [Route("api/[controller]")]
    public class DublinBikesController : Controller
    {
        private readonly PipelineHandlerCollection _handlers;
        private readonly string _apiKey;

        public DublinBikesController(IConfiguration configuration, PipelineHandlerCollection pipelineHandlerCollection)
        {
            _handlers = pipelineHandlerCollection;
            _apiKey = configuration.GetValue<string>("JCDApiKey");
        }

        private PipelineRequest GetStationsPipeline() =>
            new PipelineRequest()
                .AddHandler(_handlers.Get<SetEndpoint>())
                .AddHandler<ContextQueryParameters>(handler =>
                    handler.Add("contract")
                           .Add("apiKey"))
                .AddHandler(_handlers.Get<InvokeGetRequest>())
                .AddHandler<ParseStationsResponse>()
                .AddHandler<OrderStationsResponse>();

        [HttpGet]
        public async Task<IActionResult> GetStations([FromQuery]string orderBy)
        {
            var context = await new PipelineRequest()
                .AddHandler<GenerateObjectResult<IEnumerable<DublinBikeStation>>>()
                .AddHandlers(GetStationsPipeline())
                .InvokeAsync(ctx => ctx
                    .Add("endpoint", "stations")
                    .Add("contract", "Dublin")
                    .Add("apiKey", _apiKey)
                    .Add("orderBy", orderBy, !string.IsNullOrEmpty(orderBy))
                );

            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);
        }

        private PipelineRequest GetStationPipeline() =>
            new PipelineRequest()
                .AddHandler(_handlers.Get<SetEndpoint>())
                .AddHandler<ContextQueryParameters>(handler =>
                    handler.Add("contract")
                           .Add("apiKey"))
                .AddHandler(_handlers.Get<InvokeGetRequest>())
                .AddHandler<ParseStationResponse>();

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetStation([FromRoute]int stationId)
        {
            var context = await new PipelineRequest()
                .AddHandler<GenerateObjectResult<DublinBikeStation>>()
                .AddHandlers(GetStationPipeline())
                .InvokeAsync(ctx => ctx
                    .Add("endpoint", $"stations/{stationId}")
                    .Add("contract", "Dublin")
                    .Add("apiKey", _apiKey));

            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);
        }
        
    }
}
