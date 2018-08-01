using Tumble.Core;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tumble.Handlers.Redirection
{
    public class HostRedirect : IPipelineHandler
    {              
        public string RedirectHost { get; set; }
        public int RedirectPort { get; set; }
                
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpRequestMessage httpRequestMessage))
            {
                UriBuilder builder = new UriBuilder(httpRequestMessage.RequestUri)
                {
                    Host = RedirectHost,
                    Port = RedirectPort
                };
                var redirectUri = builder.Uri;
                context.Add(new Redirected(httpRequestMessage, builder.Uri));
                httpRequestMessage.RequestUri = redirectUri;
                
                await next.Invoke();
            }
            else
                throw new PipelineDependencyException<HttpRequestMessage>(this);
        }
    }    
}
