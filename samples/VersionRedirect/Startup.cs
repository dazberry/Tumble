using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tumble.Handlers.Miscellaneous;
using Tumble.Middleware;
using VersionRedirect.Handlers;

namespace VersionRedirect
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var config = new PipelineMiddlewareConfiguration()
            {
                AfterPipelineInvoke = PipelineMiddlewareEnum.Continue
            };

            app.UseProxyMiddleware(req =>
            {
                req.AddHandler<VersionHandler>()
                   .AddHandler<RedirectHandler>();                   

            }, config);

            app.UseMvc();
        }
    }
}
