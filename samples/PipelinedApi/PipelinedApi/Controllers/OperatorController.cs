using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Models;
using Tumble.Client.Extensions;
using Tumble.Core;
using Tumble.Handlers.Miscellaneous;

namespace PipelinedApi.Controllers
{
    [Route("api/[controller]")]
    public class OperatorController : Controller
    {
        private readonly PipelineHandlerCollection _handlers;
        private readonly Uri _baseUrl;

        public OperatorController(IConfiguration configuration, PipelineHandlerCollection pipelineHandlerCollection)
        {
            _baseUrl = configuration.GetValue<Uri>("Endpoints:baseUrl");
            _handlers = pipelineHandlerCollection;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {           
            var context = await new PipelineRequestBuilder(_handlers)              
              .AddHandler<SetEndpoint>()
              .AddHandler<InvokeGetRequest>()
              .AddHandler<ParseSuccessResponse<OperatorInformation>>()
              .PipelineRequest
              .InvokeAsync(ctx =>
                  ctx.Add("endpoint", "/operatorinformation")
              );

            var result = context.Get<ApiResponse<OperatorInformation>>();
            return Ok(result);
        }
    }
}
