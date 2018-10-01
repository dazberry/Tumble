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

    public interface IQueryParameter
    {       
        string Name { get; }        
        bool Optional { get; }

        string Value { get; }
        bool HasValue { get; }

        KeyValuePair<string, string> ToKeyValuePair();
    }

    public class QueryParameter : IQueryParameter
    {
        public string Name { get; }        
        public bool Optional { get; }

        public virtual string Value { get; }
        public bool HasValue => !string.IsNullOrEmpty(Value);

        public KeyValuePair<string, string> ToKeyValuePair() =>
            new KeyValuePair<string, string>(Name, Value);

        public QueryParameter(string name, string value, bool optional = false)
        {
            Name = name;
            Value = value;
            Optional = optional;
        }
    }

    public class ContextQueryParameter : QueryParameter
    {        
        private readonly string _contextVariableName;
              
        public ContextQueryParameter(string name, string contextVariableName, bool optional = false)
            : base(name, string.Empty, optional)
        {            
            _contextVariableName = contextVariableName;
        }
    }

    public class SetQueryParameters : IPipelineHandler
    {
        private IList<KeyValuePair<string, string>> NameValueToKeyValuePairs(NameValueCollection source) =>
            source.AllKeys.SelectMany(source.GetValues, (k, v) => new KeyValuePair<string, string>(k, v)).ToList();

        private IList<IQueryParameter> _queryParameters = new List<IQueryParameter>();

        public SetQueryParameters AddParameter(string name, string value, bool optional = false)
        {
            _queryParameters.Add(new QueryParameter(name, value, optional));
            return this;
        }

        public SetQueryParameters AddContextParameter(string parameterName, string contextVariableName, bool optional = false)
        {            
            _queryParameters.Add(new ContextQueryParameter(parameterName, contextVariableName, optional));
            return this;
        }                          

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (_queryParameters.Any() && context.GetFirst(out Uri uri))
            {
                var keyValuePairs = NameValueToKeyValuePairs(HttpUtility.ParseQueryString(uri.Query));
                foreach (var queryParameter in _queryParameters)
                {                    
                    //[todo] FetchParameterFromContext
                    if (queryParameter.HasValue)
                    {
                        keyValuePairs.Add(queryParameter.ToKeyValuePair());
                    }
                    else
                    if (!queryParameter.Optional && !queryParameter.HasValue)
                    {
                        context.AddNotification(this, $"Query Parameter \"{queryParameter.Name}\" is not optional and does not contain a value.");
                        return;
                    }
                }

                

                var ub = new UriBuilder(uri);
                QueryBuilder qb = new QueryBuilder(keyValuePairs);
                ub.Query = qb.ToString();
                context.AddOrReplace(ub.Uri);

                await next.Invoke();
            }
           
        }
    }
}
