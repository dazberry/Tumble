using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tumble.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PipelinedApi.Controllers
{
    [Route("api/[controller]")]
    public class BusstopController : Controller
    {        
        [HttpGet]
        public async Task<IActionResult> GetByStopNumber(string stopNo)
        {
            return Ok();                
        }

        public async Task<IActionResult> GetRouteStops(string routeNo)
        {
            return Ok();
        }

        public async Task<IActionResult> GetStopTimes(string stopNo)
        {
            return Ok();
        }
    }
}
