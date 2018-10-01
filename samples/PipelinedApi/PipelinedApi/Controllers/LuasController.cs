using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Models;
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
            var context = await new PipelineRequestBuilder(_handlers)
                .AddHandler<Handlers.Luas.SetEndpoint>()
                .AddHandler<Handlers.Luas.SetEncrypt>((_, ctx) =>
                    ctx.Add("encrypt", "false"))
                .AddHandler<Handlers.Luas.SetAction>((_, ctx) =>
                    ctx.Add("action", "list"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<Handlers.Luas.ParseListResponse>()
                .InvokeAsync();

            var response = context.Get<LuasLines>("response");
            return Ok(response);
        }

        [HttpGet("{stopId}")]
        public async Task<IActionResult> GetStopInfo([FromRoute] string stopId)
        {
            var context = await new PipelineRequestBuilder(_handlers)
                .AddHandler<Handlers.Luas.SetEndpoint>()
                .AddHandler<Handlers.Luas.SetEncrypt>((_, ctx) =>
                    ctx.Add("encrypt", "false"))
                .AddHandler<Handlers.Luas.SetAction>((_, ctx) =>
                    ctx.Add("action", "forecast"))
                .AddHandler<Handlers.Luas.SetStop>((_, ctx) => 
                    ctx.Add("stop", stopId))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<Handlers.Luas.ParseStopResponse>()
                .InvokeAsync();

            var response = context.Get<LuasStop>("response");
            return Ok(response);
        }
    }

}
