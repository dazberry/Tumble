using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Tumble.Client.Parameters;
using Tumble.Core;
using Tumble.Core.Contexts;

namespace Tumble.Client.Handlers
{
    /// <summary>    
    /// Sets Context.Uri from BaseUrl
    /// <para></para>        
    /// Requires: IContextWriter&lt;Uri&gt;
    /// </summary>     
    public class QueryParametersHander : IPipelineHandler<IQueryParameters, IContextResolver<Uri>>
    {
        private QueryParameters _queryParameters = new QueryParameters();

        public QueryParametersHander Add(string name, string value = "", bool optional = false)
        {
            _queryParameters.Add(name, value, optional);
            return this;
        }

        public static IEnumerable<KeyValuePair<string, string>> NameValueToKeyValuePairs(NameValueCollection source) =>
            source.AllKeys.SelectMany(source.GetValues, (k, v) => new KeyValuePair<string, string>(k, v));

        public async Task InvokeAsync(PipelineDelegate next, IQueryParameters queryParameters, IContextResolver<Uri> uri)
        {
            var handlerParameters = _queryParameters.Get();
            var contextParameters = queryParameters.Get();


            handlerParameters = handlerParameters.Concat(contextParameters)
                .GroupBy(x => x.Name)
                .Select(x => new QueryParameter()
                {
                    Name = x.Key,
                    Optional = x.Where(y => y.Optional == true)
                                .Any(),
                    Value = x.Where(y => !string.IsNullOrEmpty(y.Value))
                                .FirstOrDefault()?.Value
                });                
            

            if (handlerParameters.Any())
            {
                var emptyMandatoryParameters = handlerParameters.Where(x => !x.Optional && string.IsNullOrEmpty(x.Value));
                if (emptyMandatoryParameters.Any())
                {
                    var names = string.Join(", ", emptyMandatoryParameters.Select(x => x.Name));
                    throw new Exception($"Empty mandatory parameters: {names}");
                }

                var kvp = handlerParameters
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Select(x =>
                        new KeyValuePair<string, string>(x.Name, x.Value));               

                kvp.Concat(NameValueToKeyValuePairs(
                    HttpUtility.ParseQueryString(uri.Get().Query)));
                                                                  
                var ub = new UriBuilder(uri.Get());
                QueryBuilder qb = new QueryBuilder(kvp);
                ub.Query = qb.ToString();
                uri.Set(ub.Uri);                               
            }

            await next.Invoke();
        }
    }
}
