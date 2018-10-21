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

        private PipelineRequest GetOperatorInformationPipeline() =>
            new PipelineRequest()
                .AddHandler<ContextParameters>(
                    handler => handler
                        .Add("endpoint", "/operatorinformation"))
                .AddHandler(_handlers.Get<SetEndpoint>())
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseSuccessResponse<OperatorInformation>>();

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var context = await new PipelineRequest()
                .AddHandler<GenerateObjectResult<ApiResponse<OperatorInformation>>>()
                .AddHandlers(GetOperatorInformationPipeline())
                .InvokeAsync();

            if (context.GetFirst(out IActionResult response))
                return response;

            return new StatusCodeResult(500);
        }
    }
}
