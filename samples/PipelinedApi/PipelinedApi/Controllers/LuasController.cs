using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Handlers.Luas;
using PipelinedApi.Models;
using PipelinedApi.Models.Extensions;
using Tumble.Client.Handlers;
using Tumble.Core;
using Tumble.Core.Handlers;

namespace PipelinedApi.Controllers
{
    [Route("api/[controller]")]
    public class LuasController : Controller
    {
        private readonly PipelineHandlerCollection _handlers;

        public LuasController(IConfiguration configuration, PipelineHandlerCollection pipelineHandlerCollection)
        {
            _handlers = pipelineHandlerCollection;
        }
               
        private PipelineRequest GetLuasStopListPipeline() =>
            new PipelineRequest()
                .AddHandler(_handlers.Get<Handlers.Luas.SetEndpoint>())
                .AddHandler<QueryParameters>(
                    handler => handler.Add("encrypt", "false")
                                      .Add("action", "list"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ValidateLuasResponse>()
                .AddHandler<ParseListResponse>();                
             
        [HttpGet("list")]
        public async Task<IActionResult> GetStopList()
        {
            var context = await new PipelineRequest()
                .AddHandler<GenerateObjectResult<LuasLines>>()
                .AddHandlers(GetLuasStopListPipeline())
                .InvokeAsync();

            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);
        }

        private PipelineRequest GetStopInfoPipeline() =>
            new PipelineRequest()
                .AddHandler(_handlers.Get<Handlers.Luas.SetEndpoint>())
                .AddHandler<QueryParameters>(
                    handler => handler.Add("encrypt", "false")
                                      .Add("action", "forecast"))
                .AddHandler<ContextQueryParameters>(
                    handler => handler.Add("stop"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ValidateLuasResponse>()
                .AddHandler<ParseStopResponse>();

        [HttpGet("{stopId}")]
        public async Task<IActionResult> GetStopInfo([FromRoute] string stopId)
        {
            var context = await new PipelineRequest()
                .AddHandler<GenerateObjectResult<LuasStop>>()
                .AddHandlers(GetStopInfoPipeline())
                .InvokeAsync(ctx => ctx
                    .Add("stop", stopId));

            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);            
        }

        [HttpGet("{line}/{direction}")]
        public async Task<IActionResult> GetAllByLineAndDirection([FromRoute] string line, [FromRoute] string direction)
        {
            string[] directions = { "inbound", "outbound" };

            if (!directions.Any(x => string.Compare(x, direction, true) == 0))
                return BadRequest(new { error = "Invalid direction specified", directions });

            var context = await GetLuasStopListPipeline().InvokeAsync();
            if (!context.GetFirst(out LuasLines stopList))            
                return BadRequest(new { error = "Error retrieving stop info" });
                                        
            var index = stopList.GetIndexOfShortName(line);
            if (index == -1)
                return BadRequest(new { error = "Invalid line specified", lines = stopList.GetLineShortNames() });

            var names = stopList.Line[index].Stops.Select(x => x.Abrev).ToArray();

            var pipeline = new PipelineRequest()
                .AddHandler<ConcurrentPipelines>(handler =>
                {
                    foreach (var name in names)
                        handler.Add(GetStopInfoPipeline(),
                                    new PipelineContext().Add("stop", name));
                });

            context = await pipeline.InvokeAsync();
            var contexts = context.Get<PipelineContext>();

            List<object> objects = new List<object>();
            foreach (var ctx in contexts)
            {
                var luasStops = ctx.Get<LuasStop>("response");
                var res = new
                {
                    stop = luasStops.Stop,
                    stopAbv = luasStops.StopAbv,
                    direction,
                    trams = luasStops.Direction
                                .Where(x => string.Compare(x.Name, direction, true) == 0)
                                .Select(x =>
                                    x.Tram.Select(
                                        y => new
                                        {
                                            dueMins = y.DueMins,
                                            destination = y.Destination
                                        }))
                };
                objects.Add(res);
            }

            return Ok(objects.ToArray());
        }
    }

}
