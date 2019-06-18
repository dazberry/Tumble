using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Proxy
{
    /// <summary>
    /// Requires HttpRequestMessage
    /// </summary>
    public class HostRedirectHandler : IPipelineHandler<HttpRequestMessage>
    {
        public string RedirectHost { get; set; }
        public int RedirectPort { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, HttpRequestMessage httpRequestMessage)
        {
            UriBuilder builder = new UriBuilder(httpRequestMessage.RequestUri)
            {
                Host = RedirectHost,
                Port = RedirectPort
            };
            var redirectUri = builder.Uri;
            httpRequestMessage.RequestUri = redirectUri;

            await next.Invoke();
        }
    }
}
