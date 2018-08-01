using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tumble.Core;

namespace Tumble.Handlers.Monitoring
{
    public class ResponseLogger : IPipelineHandler
    {
        private readonly ILogger _logger;

        public ResponseLogger(ILogger logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            await next.Invoke();

            if (context.GetFirst(out HttpResponseMessage httpResponseMessage))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"$RESPONSE: [{DateTime.UtcNow.ToString("u")}] {context.Id}{Environment.NewLine}" +
                            $"{httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}{Environment.NewLine}" +
                            $" [Headers]");
                foreach (var header in httpResponseMessage.Headers)
                    sb.AppendLine($" {header.Key} : {header.Value}");

                _logger.LogInformation(sb.ToString());
            }
            else
            {
                _logger.LogInformation($"$NORESPONSE: [{DateTime.UtcNow.ToString("u")}] {context.Id}");                
            }
        }
    }
}
