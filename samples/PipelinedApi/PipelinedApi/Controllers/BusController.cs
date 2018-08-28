using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Models;
using PipelinedApi.Pipelines;
using PipelinedApi.Routes.Common;
using Tumble.Core;
using Tumble.Handlers.Miscellaneous;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PipelinedApi.Controllers
{
    [Route("api/[controller]")]
    public class BusController : Controller
    {
        private readonly IConfiguration _configuration;

        public BusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("stops/{stopId}")]
        public async Task<IActionResult> GetByStopNumber([FromRoute]int stopId)
        {
            var request = ByStopNumberPipeline.Get();
            var result = await request
                .InvokeAsync(ctx => 
                    ctx.Add("stopId", stopId)
                       .Add("endpoint", _configuration["Endpoints:ByStopNumber"])
                );
            if (result.Get("response", out ApiResponse<ArrivalInfo> response))
                return Ok(response);
            return BadRequest();
        }

        [HttpGet("Route")]
        public async Task<IActionResult> GetRouteStops(string routeNo)
        {
            return Ok();
        }

        [HttpGet("StopTimes")]
        public async Task<IActionResult> GetStopTimes(string stopNo)
        {
            return Ok();
        }
    }
}
