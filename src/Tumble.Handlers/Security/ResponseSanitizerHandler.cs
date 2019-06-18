using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Extensions;
using Tumble.Handlers.Security.Contexts;

namespace Tumble.Handlers.Security
{
    public class ResponseSanitizerHandler : IPipelineHandler<HttpRequestMessage, IResponseSanitizerContext>
    {
        public bool Sanitize500s { get; set; } = true;
        public bool Sanitize400s { get; set; } = true;
        public bool Sanitize300s { get; set; } = true;
        public IList<int> SanitizeResponseCodes { get; set; } = new List<int>();

        public int? ReplacementResponseCode { get; set; } = null;

        public bool ReturnErrorReference { get; set; }
        public bool SaveOriginalContent { get; set; }

        private bool Between(int value, int low, int high) =>
            value >= low & value <= high;

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage, IResponseSanitizerContext context)
        {
            await next.Invoke();

            var resp = context.OriginalResponse;
            if (resp != null)
            {
                var code = (int)resp.StatusCode;
                if ((Sanitize500s && Between(code, 500, 599)) ||
                    (Sanitize400s && Between(code, 400, 499)) ||
                    (Sanitize300s && Between(code, 300, 399)) ||
                    SanitizeResponseCodes.Contains(code))
                {
                    if (SaveOriginalContent)
                        context.OriginalResponse = resp;

                    if (!context.ErrorReference.HasValue)
                        context.ErrorReference = Guid.NewGuid();

                    HttpResponseBuilder.HttpResponseMessage(resp)
                        .WithNoContent()
                        .WithReasonPhrase(string.Empty)
                        .If(ReturnErrorReference,
                            r => r.WithStringContent($"{{\"errorReference\":\"{context.ErrorReference}\"}}"))
                        .If(ReplacementResponseCode.HasValue,
                            r => r.WithStatusCode(ReplacementResponseCode.Value));
                }
            }
        }
        
    }
}
