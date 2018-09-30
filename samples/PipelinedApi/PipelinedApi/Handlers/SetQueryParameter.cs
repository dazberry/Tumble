using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Handlers
{
    public class SetQueryParameter : IPipelineHandler
    {
        private readonly string _contextVariableName;
        private readonly string _queryParameterName;

        private bool _optionalParameter;
        public bool OptionalParameter
        {
            get
            {
                return _optionalParameter;
            }
            set
            {
                _optionalParameter = value;
            }
        }

        public void MakeOptional() =>
            _optionalParameter = true;

        private IList<KeyValuePair<string, string>> NameValueToKeyValuePairs(NameValueCollection source) =>
            source.AllKeys.SelectMany(source.GetValues, (k, v) => new KeyValuePair<string, string>(k, v)).ToList();

        public SetQueryParameter(string contextVariableName, string queryParameterName, bool optionalParameter = false)
        {
            _contextVariableName = contextVariableName;
            _queryParameterName = queryParameterName;
            _optionalParameter = optionalParameter;
        }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out Uri uri))
            {
                if (context.Get(_contextVariableName, out string value) && value != null)
                {
                    var keyValuePairs = NameValueToKeyValuePairs(HttpUtility.ParseQueryString(uri.Query));                    
                    keyValuePairs.Add(new KeyValuePair<string, string>(_queryParameterName, value));

                    var ub = new UriBuilder(uri);
                    QueryBuilder qb = new QueryBuilder(keyValuePairs);
                    ub.Query = qb.ToString();
                    context.AddOrReplace(ub.Uri);
                }
                else
                {
                    if (!_optionalParameter)
                    {
                        context.AddNotification(this, $"Context parameter {_contextVariableName} not found");
                        return;
                    }
                }

                await next.Invoke();
            }
            else
                context.AddNotification(this, "Uri not found");            
        }
    }
}
