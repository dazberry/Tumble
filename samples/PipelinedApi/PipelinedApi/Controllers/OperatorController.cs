using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelinedApi.Handlers;
using PipelinedApi.Models;
using PipelinedApi.Handlers.Rtpi;
using Tumble.Core;
using PipelinedApi.Handlers.RTPI;
using Tumble.Client.Handlers;
using PipelinedApi.Contexts;

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

        private PipelineRequest GetOperatorInformationPipeline(string routeSegment) =>
            new PipelineRequest()
                .AddHandler(_handlers.Get<SetRTPIEndpoint>())
                .AddHandler<AppendRouteHandler>(handler => 
                    handler.Route = routeSegment)
                .AddHandler<QueryParametersHander>(
                    handler => handler
                        .Add("format", "jsom"))
                .AddHandler<InvokeGetRequest>()
                .AddHandler<ParseSuccessResponse<ApiResponse<OperatorInformation>>>();            

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var context = await GetOperatorInformationPipeline("/operatorinformation")                    
                    .AddHandler<GenerateObjectResult<ApiResponse<OperatorInformation>>>()
                    .InvokeAsync(new RTPIContext<ApiResponse<OperatorInformation>>());

            return context.ObjectResult;
        }
    }
}
