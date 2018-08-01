using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Tumble.Core;
using System.Linq;

namespace Tumble.Handlers.Redirection
{
    public enum ParamEvaluationEnum { ParamExists, ParamDoesNotExist, ParamValueMatches, ParamValueDoesNotMatch }

    public class ParamRedirect : IPipelineHandler
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public ParamEvaluationEnum ParamEvaluation { get; set; } = ParamEvaluationEnum.ParamValueMatches;

        public string RedirectUrl { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (!context.GetFirst(out HttpRequestMessage httpRequestMessage))
                throw new PipelineDependencyException<HttpRequestMessage>(this);
                       
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
                var redirectedUri = builder.Uri;
                context.Add(new Redirected(httpRequestMessage, redirectedUri));
                httpRequestMessage.RequestUri = redirectedUri;                
            }

            await next.Invoke();
        }
    }
}
