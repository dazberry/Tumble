using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tumble.Core;

namespace Tumble.Handlers.Monitoring
{
    public class RequestLogger : IPipelineHandler
    {
        ILogger _logger;
        public RequestLogger(ILogger logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpRequestMessage httpRequestMessage))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"REQUEST: [{DateTime.UtcNow.ToString("u")}] {context.Id}{Environment.NewLine}" +
                          $"[{httpRequestMessage.Method.ToString()}] {httpRequestMessage.RequestUri.ToString()} {Environment.NewLine}" +
                          $" [Headers]");
                foreach (var header in httpRequestMessage.Headers)
                    sb.AppendLine($" {header.Key} : {header.Value}");

                _logger.LogInformation(sb.ToString());
                                    
                await next.Invoke();                
            }
            else
                throw new PipelineDependencyException<HttpRequestMessage>(this);           
        }
    }
}
