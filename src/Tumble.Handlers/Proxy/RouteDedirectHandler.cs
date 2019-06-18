using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tumble.Client;
using Tumble.Core;
using Tumble.Core.Contexts;
using Tumble.Handlers.Contexts;

namespace Tumble.Handlers.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteRedirectHandler : IPipelineHandler<HttpRequestMessage, IContextResolver<HttpResponseMessage>>
    {
        public IDictionary<string, string> Redirects { get; set; }
        public bool Return404IfNotFound { get; set; } = true;


        private bool Match(string sourcePath, string matchPath)
        {
            return string.Compare(sourcePath, matchPath, true) == 0;
        }

        private Uri FindMatchingRedirect(Uri source)
        {
            var result = Redirects
                .Select(x => new { key = x.Key, value = x.Value })
                .FirstOrDefault(redirect =>
                {
                    var path = HttpUtility.UrlDecode(source.AbsolutePath);
                    return Match(path, redirect.key);
                });

            if (result != null)
            {
                var builder = new UriBuilder(result.value)
                {
                    Query = source.Query
                };
                return builder.Uri;
            }
            return null;
        }

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage, IContextResolver<HttpResponseMessage> httpResponseMessage)
        {                       
            var redirectUri = FindMatchingRedirect(httpRequestMessage.RequestUri);
            if (redirectUri != null)
                httpRequestMessage.RequestUri = redirectUri;
            else
            {
                if (Return404IfNotFound)
                {
                    var response = HttpResponseBuilder
                        .HttpResponseMessage()
                        .WithStatusCode(404)
                        .WithContent(new StringContent(
                            "{\"RouteRedirect\": \"No matching route\"}",
                            Encoding.ASCII,
                            "appliation/json"))
                        .Build();

                    httpResponseMessage.Set(response);
                    return;
                }
            }
            await next.Invoke();            
        }

    }
}
