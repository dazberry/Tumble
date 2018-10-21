using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
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
                .AddHandler<Handlers.Luas.ParseListResponse>();                
             
        private async Task<LuasLines> GetStopListAsync()
        {
            var context = await new PipelineRequest()
                .AddHandlers(GetLuasStopListPipeline())
                .InvokeAsync();

            return context.Get<LuasLines>("response");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetStopList()
        {
            var response = await GetStopListAsync();
            return Ok(response);
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
                .AddHandler<Handlers.Luas.ParseStopResponse>();

        [HttpGet("{stopId}")]
        public async Task<IActionResult> GetStopInfo([FromRoute] string stopId)
        {
            var context = await new PipelineRequest()
                .AddHandlers(GetStopInfoPipeline())
                .InvokeAsync(ctx => ctx
                    .Add("stop", stopId));            
           
            var response = context.Get<LuasStop>("response");
            return Ok(response);
        }

        [HttpGet("{line}/{direction}")]
        public async Task<IActionResult> GetAllByLineAndDirection([FromRoute] string line, [FromRoute] string direction)
        {
            string[] directions = { "inbound", "outbound" };

            if (!directions.Any(x => string.Compare(x, direction, true) == 0))
                return BadRequest(new { error = "Invalid direction specified", directions });

            var stopList = await GetStopListAsync();
                
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

            var context = await pipeline.InvokeAsync();
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
