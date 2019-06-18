using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Tumble.Core;

namespace Tumble.Handlers.Proxy
{
    public enum ParamEvaluationEnum { ParamExists, ParamDoesNotExist, ParamValueMatches, ParamValueDoesNotMatch }

    /// <summary>
    /// Requires HttpRequestMessage
    /// </summary>
    public class ParamRedirectHandler : IPipelineHandler<HttpRequestMessage>
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public ParamEvaluationEnum ParamEvaluation { get; set; } = ParamEvaluationEnum.ParamValueMatches;

        public string RedirectUrl { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage)
        {
            var parameters = HttpUtility.ParseQueryString(httpRequestMessage.RequestUri.Query);
            var paramNameAndValue = parameters.Cast<string>()
                .Where(key => string.Compare(key, ParamName, true) == 0)
                .Select(key => new { key, value = parameters[key] })
                .FirstOrDefault();

            var matched = false;
            switch (ParamEvaluation)
            {
                case ParamEvaluationEnum.ParamExists:
                    matched = paramNameAndValue != null;
                    break;
                case ParamEvaluationEnum.ParamDoesNotExist:
                    matched = paramNameAndValue == null;
                    break;
                case ParamEvaluationEnum.ParamValueMatches:
                    matched = string.Compare(paramNameAndValue?.value, ParamValue, true) == 0;
                    break;
                case ParamEvaluationEnum.ParamValueDoesNotMatch:
                    matched = (paramNameAndValue != null) &&
                        string.Compare(paramNameAndValue?.value, ParamValue, true) != 0;
                    break;
            }

            if (matched)
            {
                UriBuilder builder = new UriBuilder(RedirectUrl)
                {
                    Query = httpRequestMessage.RequestUri.Query
                };
                httpRequestMessage.RequestUri = builder.Uri;
            }

            await next.Invoke();
        }
    }
}
