using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Tumble.Client.Handlers;
using Tumble.Handlers;
using Tumble.Handlers.Redirection;
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
                   .AddHandler<HostRedirect>(handler =>
                   {
                       handler.RedirectHost = "localhost";
                       handler.RedirectPort = 50596;
                   })
                   .AddHandler<ClientRequest>(handler =>
                   {
                       handler.RequestTimeout = TimeSpan.FromSeconds(60);
                   });
                   
            });
        }
    }
}
