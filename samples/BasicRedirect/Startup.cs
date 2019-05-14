using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tumble.Client.Handlers;
using Tumble.Handlers.Proxy;
using Tumble.Middleware;

namespace BasicRedirect.Web.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseProxyMiddleware(req =>
            {                
                req.AddHandler<HttpRequestHandler>()                   
                   .AddHandler<HttpResponseHandler>()
                   .AddHandler<HostRedirectHandler>(handler =>
                   {
                       handler.RedirectHost = "localhost";
                       handler.RedirectPort = 50596;
                   })
                   .AddHandler<ClientRequestHandler>(handler =>
                   {                       
                       handler.RequestTimeout = TimeSpan.FromSeconds(60);
                   });

            });
        }
    }
}
