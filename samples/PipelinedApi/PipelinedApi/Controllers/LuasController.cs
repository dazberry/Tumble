using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Contexts;
using PipelinedApi.Handlers;
using PipelinedApi.Handlers.Luas;
using PipelinedApi.Models;
using PipelinedApi.Models.Extensions;
using Tumble.Client.Handlers;
using Tumble.Client.Parameters;
using Tumble.Core;
using Tumble.Handlers.Miscellaneous;

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
                .AddHandler(_handlers.Get<SetLuasEndpoint>())
                .AddHandler<QueryParametersHander>(
                    handler => handler.Add("encrypt", "false")
                                      .Add("action", "list"))
                .AddHandler<InvokeGetRequest>()                
                .AddHandler<ParseListResponse>();                

            
        [HttpGet("list")]
        public async Task<IActionResult> GetStopList()
        {
            var context = await GetLuasStopListPipeline()
               .AddHandler<GenerateObjectResult<LuasLines>>()
               .InvokeAsync(new LuasContext<LuasLines>());

            return context.ObjectResult;
        }

        private PipelineRequest GetStopInfoPipeline() =>
            new PipelineRequest()
                .AddHandler(_handlers.Get<SetLuasEndpoint>())
                .AddHandler<QueryParametersHander>(
                    handler => handler
                                .Add("encrypt", "false")
                                .Add("action", "forecast")
                                .Add("stop"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseStopResponse>();

        [HttpGet("{stopId}")]
        public async Task<IActionResult> GetStopInfo([FromRoute] string stopId)
        {
            var context = await GetStopInfoPipeline()
                .AddHandler<GenerateObjectResult<LuasStop>>()
                .InvokeAsync(new LuasContext<LuasStop>(), ctx => ctx
                    .Add("stop", stopId));

            return context.ObjectResult;
        }

        [HttpGet("{line}/{direction}")]
        public async Task<IActionResult> GetAllByLineAndDirection([FromRoute] string line, [FromRoute] string direction)
        {           
            string[] directions = { "inbound", "outbound" };

            if (!directions.Any(x => string.Compare(x, direction, true) == 0))
                return BadRequest(new { error = "Invalid direction specified", directions });

            var context = await GetLuasStopListPipeline()
                .InvokeAsync(new LuasContext<LuasLines>());

            var stopList = context.ApiResponse;
            if (stopList == null)            
               return BadRequest(new { error = "Error retrieving stop info" });

            var index = stopList.GetIndexOfShortName(line);
            if (index == -1)
                return BadRequest(new { error = "Invalid line specified", lines = stopList.GetLineShortNames() });

            var names = stopList.Line[index].Stops.Select(x => x.Abrev).ToArray();


            var parallelHandler = new ParallelHandler<LuasContext<LuasStop>>();
            foreach (var name in names)
                parallelHandler.Add(
                    GetStopInfoPipeline(),
                    new LuasContext<LuasStop>(),
                    ctx => ctx.Add("stop", name)
                );                

            var result = await new PipelineRequest()
                .AddHandler(parallelHandler)
                .InvokeAsync(context, null);
            
            List<object> objects = new List<object>();
            foreach (var ctx in parallelHandler.Contexts)
            {
                var luasStops = ctx.ApiResponse;
                if (luasStops != null)
                {
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
            }

            return Ok(objects.ToArray());
        }
    }

}
