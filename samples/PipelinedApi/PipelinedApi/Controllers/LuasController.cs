using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Models;
using Tumble.Client.Handlers;
using Tumble.Core;

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

        [HttpGet("list")]
        public async Task<IActionResult> GetStopList()
        {
            var context = await new PipelineRequest()
                .AddHandlerFromCollection<Handlers.Luas.SetEndpoint>(_handlers)
                .AddHandler<QueryParameters>(
                    handler => handler.Add("encrypt", "false")
                                      .Add("action", "list"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<Handlers.Luas.ParseListResponse>()
                .InvokeAsync();
                           
            var response = context.Get<LuasLines>("response");
            return Ok(response);
        }

        [HttpGet("{stopId}")]
        public async Task<IActionResult> GetStopInfo([FromRoute] string stopId)
        {
            var pipeline = new PipelineRequest()
                .AddHandlerFromCollection<Handlers.Luas.SetEndpoint>(_handlers)
                .AddHandler<QueryParameters>(
                    handler => handler.Add("encrypt", "false")
                                      .Add("action", "forecast"))
                .AddHandler<ContextQueryParameters>(
                    handler => handler.Add("stop"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<Handlers.Luas.ParseStopResponse>();

            var context = new PipelineContext()
                .Add("stop", stopId);

            await pipeline.InvokeAsync(context);
              
            var response = context.Get<LuasStop>("response");
            return Ok(response);
        }
    }

}
