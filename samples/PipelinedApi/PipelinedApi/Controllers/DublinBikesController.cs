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
                .AddHandler<ParseStationsResponse>();

        [HttpGet]
        public async Task<IActionResult> GetStations()
        {
            var context = await GetStationsPipeline()
                .InvokeAsync(ctx => ctx
                    .Add("endpoint", "stations")
                    .Add("contract", "Dublin")
                    .Add("apiKey", _apiKey));
            
            return Ok(context.Get<IEnumerable<DublinBikeStation>>("response"));
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
            var context = await GetStationPipeline()
                .InvokeAsync(ctx => ctx
                    .Add("endpoint", $"stations/{stationId}")
                    .Add("contract", "Dublin")
                    .Add("apiKey", _apiKey));
           
            return Ok(context.Get<DublinBikeStation>("response"));
        }
        
    }
}
