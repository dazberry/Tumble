using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Tumble.Client.Contexts;
using Tumble.Client.Parameters;
using Tumble.Core;

namespace Tumble.Client.Handlers
{      
        
    public class QueryParametersHander : IPipelineHandler<IHttpClientContext>
    {
        private QueryParameters _queryParameters = new QueryParameters();

        public QueryParametersHander Add(string name, string value = "", bool optional = false)
        {
            _queryParameters.Add(name, value, optional);
            return this;
        }

        public static IEnumerable<KeyValuePair<string, string>> NameValueToKeyValuePairs(NameValueCollection source) =>
            source.AllKeys.SelectMany(source.GetValues, (k, v) => new KeyValuePair<string, string>(k, v));

        public async Task InvokeAsync(PipelineDelegate next, IHttpClientContext context)
        {
            var handlerParameters = _queryParameters.Get();
            var contextParameters = context.QueryParameters.Get();

            if (handlerParameters.Any())
            {
                var parameters = handlerParameters.GroupJoin(
                    contextParameters,
                    hp => hp.Name,
                    cp => cp.Name,
                    (hp, cp) =>
                        cp.FirstOrDefault(x => !string.IsNullOrEmpty(x.Value)) ?? hp                        
                    );

                if (parameters.Any(x => !x.Optional && string.IsNullOrEmpty(x.Value)))
                    throw new Exception(""); //todo

                var keyValuePairs = NameValueToKeyValuePairs(
                    HttpUtility.ParseQueryString(context.Uri.Query));

                keyValuePairs = keyValuePairs.GroupJoin(
                    parameters
                        .Where(x => !string.IsNullOrEmpty(x.Value)),
                    kvp => kvp.Key,
                    pm => pm.Value,
                    (kvp, pm) =>
                    {
                        if (pm?.Any() ?? false)
                            return new KeyValuePair<string, string>(pm.First().Name, pm.First().Value);
                        return kvp;
                    });
                
                var ub = new UriBuilder(context.Uri);
                QueryBuilder qb = new QueryBuilder(keyValuePairs);
                ub.Query = qb.ToString();
                context.Uri = ub.Uri;
                
                await next.Invoke();
            }

        }
    }
}
