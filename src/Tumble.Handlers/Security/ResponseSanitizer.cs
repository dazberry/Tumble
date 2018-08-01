using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Extensions;

namespace Tumble.Handlers.Security
{
    public class ResponseSanitizerContent
    {        
        public HttpStatusCode OriginalStatusCode { get; set; }
        public HttpContent Content { get; set; }

        public void Copy(HttpResponseMessage httpResponseMessage)
        {

        }
    }

    public class ResponseSanitizer : IPipelineHandler
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

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            await next.Invoke();
            if (context.GetFirst(out HttpResponseMessage httpResponseMessage))
            {
                var code = (int)httpResponseMessage.StatusCode;
                if ((Sanitize500s && Between(code, 500, 599)) ||
                    (Sanitize400s && Between(code, 400, 499)) ||
                    (Sanitize300s && Between(code, 300, 399)) ||
                    SanitizeResponseCodes.Contains(code))
                {                    
                    if (SaveOriginalContent)
                    {
                        var originalResponse = new ResponseSanitizerContent();
                        originalResponse.Copy(httpResponseMessage);
                        context.Add(originalResponse);                        
                    }

                    HttpResponseBuilder.HttpResponseMessage(httpResponseMessage)
                        .WithNoContent()
                        .WithReasonPhrase(string.Empty)
                        .If(ReturnErrorReference,
                            resp => resp.WithStringContent($"{{\"errorReference\":\"{context.Id}\"}}"))
                        .If(ReplacementResponseCode.HasValue,
                            resp => resp.WithStatusCode(ReplacementResponseCode.Value));
                }
            }            
        }
    }
}
