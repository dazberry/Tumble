using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace Tumble.Client.Handlers
{
    public class QueryParameters : IPipelineHandler
    {
        private class QueryParameter
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public bool Optional { get; set; }
        }

        private IList<KeyValuePair<string, string>> NameValueToKeyValuePairs(NameValueCollection source) =>
            source.AllKeys.SelectMany(source.GetValues, (k, v) => new KeyValuePair<string, string>(k, v)).ToList();

        private IList<QueryParameter> _queryParameters = new List<QueryParameter>();

        public QueryParameters Add<T>(string name, T value, bool optional = false)
        {
            _queryParameters.Add(
                new QueryParameter()
                {
                    Name = name,
                    Value = value.ToString(),
                    Optional = optional
                });
            return this;
        }

       
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (_queryParameters.Any())
            {
                if (!context.GetFirst(out Uri uri))                
                    throw new PipelineDependencyException<Uri>(this);                                    

                var keyValuePairs = NameValueToKeyValuePairs(HttpUtility.ParseQueryString(uri.Query));
                foreach (var queryParameter in _queryParameters)
                {                       
                    if (!string.IsNullOrEmpty(queryParameter.Value))
                    {                        
                        keyValuePairs.Add(new KeyValuePair<string, string>(queryParameter.Name, queryParameter.Value));
                    }
                    else
                    if (!queryParameter.Optional)
                    {
                        context.AddNotification(this, $"Query parameter \"{queryParameter.Name}\" does not contain a value and is not optional");
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
