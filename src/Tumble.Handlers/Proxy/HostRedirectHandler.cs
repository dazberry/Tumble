using System;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Contexts;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Proxy
{
    /// <summary>
    /// Requires IHttpRequestMessageContext. Sets HttpRequestMessage.RequestUri
    /// </summary>
    public class HostRedirectHandler : IPipelineHandler<IHttpRequestMessageContext>
    {
        public string RedirectHost { get; set; }
        public int RedirectPort { get; set; }

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestMessageContext context)
        {
            UriBuilder builder = new UriBuilder(context.HttpRequestMessage.RequestUri)
            {
                Host = RedirectHost,
                Port = RedirectPort
            };
            var redirectUri = builder.Uri;
            context.HttpRequestMessage.RequestUri = redirectUri;

            await next.Invoke();
        }
    }
}
