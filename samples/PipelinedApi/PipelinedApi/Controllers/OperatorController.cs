using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Models;
using PipelinedApi.Handlers.Rtpi;
using Tumble.Core;
using Tumble.Core.Notifications;
using System.Linq;

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
            var pipeline = new PipelineRequest()
              .AddHandlerFromCollection<SetEndpoint>(_handlers)
              .AddHandler<InvokeGetRequest>()
              .AddHandler<ParseSuccessResponse<OperatorInformation>>()
              .AddHandler<GenerateObjectResult<ApiResponse<OperatorInformation>>>();

            var context = new PipelineContext()
                .Add("endpoint", "/operatorinformation");

            await pipeline.InvokeAsync(context);

            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);
        }
    }
}
