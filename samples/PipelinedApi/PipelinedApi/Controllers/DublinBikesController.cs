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

        [HttpGet]
        public async Task<IActionResult> GetStations()
        {            
            var pipeline = new PipelineRequest()
                .AddHandlerFromCollection<SetEndpoint>(_handlers)
                .AddHandler<ContextQueryParameters>(handler =>
                    handler.Add("contract")
                           .Add("apiKey"))
                .AddHandlerFromCollection<InvokeGetRequest>(_handlers)
                .AddHandler<ParseStationsResponse>();
                    

            var context = new PipelineContext()
                .Add("endpoint", "stations")
                .Add("contract", "Dublin")
                .Add("apiKey", _apiKey);

            await pipeline.InvokeAsync(context);

            return Ok(context.Get<IEnumerable<DublinBikeStation>>("response"));
        }

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetStation([FromRoute]int stationId)
        {
            var pipeline = new PipelineRequest()
                .AddHandlerFromCollection<SetEndpoint>(_handlers)
                .AddHandler<ContextQueryParameters>(handler =>
                    handler.Add("contract")
                           .Add("apiKey"))
                .AddHandlerFromCollection<InvokeGetRequest>(_handlers)
                .AddHandler<ParseStationResponse>();


            var context = new PipelineContext()
                .Add("endpoint", $"stations/{stationId}")
                .Add("contract", "Dublin")
                .Add("apiKey", _apiKey);

            await pipeline.InvokeAsync(context);

            return Ok(context.Get<DublinBikeStation>("response"));
        }
        
    }
}
