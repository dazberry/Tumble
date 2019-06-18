using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Contexts;
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

        private PipelineRequest GetStationsPipeline(string route) =>
            new PipelineRequest()
                .AddHandlers(
                    _handlers.Get<SetDublinBikesEndpoint>())
                .AddHandler<RouteHandler>(handler =>
                    handler.Route = route)
                .AddHandler<QueryParametersHander>(handler =>
                    handler.Add("contract")
                           .Add("apiKey", _apiKey)
                 )
                .AddHandlers(_handlers.Get<InvokeGetRequest>());                

        [HttpGet]
        public async Task<IActionResult> GetStations([FromQuery]string orderBy)
        {
            try
            {
                var result = await GetStationsPipeline("stations")
                    .AddHandler<ParseStationsResponse>()
                    .AddHandler<OrderStationsResponse>()
                    .AddHandler<GenerateObjectResult<IEnumerable<DublinBikeStation>>>()
                    .InvokeAsync(
                        new DublinBikesContext<IEnumerable<DublinBikeStation>>(),
                        ctx => ctx
                        .Add("contract", "Dublin")
                        .Add("apiKey", _apiKey)
                        .Add("orderBy", orderBy, string.IsNullOrEmpty(orderBy)));

                return result.ObjectResult;                
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
            }                                  
        }

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetStation([FromRoute]int stationId)
        {
            var result = await GetStationsPipeline($"stations/{stationId}")
                .AddHandler<ParseStationResponse>()
                .AddHandler<GenerateObjectResult<DublinBikeStation>>()
                .InvokeAsync(
                    new DublinBikesContext<DublinBikeStation>(),
                    ctx => ctx
                        .Add("contract", "Dublin")
                        .Add("apiKey", _apiKey));

            return result.ObjectResult;
        }
        
    }
}
