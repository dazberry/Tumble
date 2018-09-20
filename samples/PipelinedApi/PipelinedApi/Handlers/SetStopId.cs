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
    public class SetStopId : IPipelineHandler
    {
        private IList<KeyValuePair<string, string>> ToKVP(NameValueCollection source) =>
            source.AllKeys.SelectMany(source.GetValues, (k, v) => new KeyValuePair<string, string>(k, v)).ToList();

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out Uri uri) && context.Get("stopId", out string stopId))
            {
                var kvp = ToKVP(HttpUtility.ParseQueryString(uri.Query));
                kvp.Add(new KeyValuePair<string, string>("stopId", stopId));                

                var ub = new UriBuilder(uri);
                QueryBuilder qb = new QueryBuilder(kvp);
                ub.Query = qb.ToString();                
                context.AddOrReplace(ub.Uri);

                await next.Invoke();
            }
            else
                context.AddNotification(this, "Uri not found");
        }
    }
}
