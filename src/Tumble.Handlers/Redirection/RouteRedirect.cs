using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tumble.Client;
using Tumble.Core;

namespace Tumble.Handlers.Redirection
{
    public class RouteRedirect : IPipelineHandler
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

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpRequestMessage httpRequestMessage))
            {                
                var redirectUri = FindMatchingRedirect(httpRequestMessage.RequestUri);
                if (redirectUri != null)
                {
                    var redirected = new Redirected(
                        httpRequestMessage, redirectUri);
                    context.Add(redirected);

                    httpRequestMessage.RequestUri = redirectUri;
                }
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
                        context.Add(response);

                        return;
                    }
                }

                await next.Invoke();                                          
            }
            else
                throw new PipelineDependencyException<HttpRequestMessage>(this);

        }
    }
}
