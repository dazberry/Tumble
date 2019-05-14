using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tumble.Core;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Proxy
{
    //class ParamRedirectHandler
    public enum ParamEvaluationEnum { ParamExists, ParamDoesNotExist, ParamValueMatches, ParamValueDoesNotMatch }

    public class ParamRedirectHandler : IPipelineHandler<IHttpRequestResponseContext>
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public ParamEvaluationEnum ParamEvaluation { get; set; } = ParamEvaluationEnum.ParamValueMatches;

        public string RedirectUrl { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestResponseContext context)
        {
            var req = context.HttpRequestMessage;

            var parameters = HttpUtility.ParseQueryString(req.RequestUri.Query);
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
                    Query = req.RequestUri.Query
                };
                req.RequestUri = builder.Uri;
            }

            await next.Invoke();
        }
    }
}
