using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PipelinedApi.Handlers;
using PipelinedApi.Handlers.DublinBikes;
using PipelinedApi.Handlers.Luas;
using PipelinedApi.Handlers.Rtpi;
using PipelinedApi.Handlers.RTPI;
using Swashbuckle.AspNetCore.Swagger;
using Tumble.Core;

namespace PipelinedApi
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
            var pipelineHandlerCollection = new PipelineHandlerCollection()
                .Add<SetRTPIEndpoint>(handler =>
                {
                    var baseUrl = Configuration.GetValue<Uri>("Endpoints:Rtpi:baseUrl");
                    handler.BaseUrl = baseUrl;
                })
                .Add<SetLuasEndpoint>(handler =>
                {
                    var baseUrl = Configuration.GetValue<Uri>("Endpoints:Luas:baseUrl");
                    handler.BaseUrl = baseUrl;
                })
                .Add<SetDublinBikesEndpoint>(handler =>
                {
                    var baseUrl = Configuration.GetValue<Uri>("Endpoints:DublinBikes:baseUrl");
                    handler.BaseUrl = baseUrl;
                })
                .Add<InvokeGetRequest>();

            services.AddSingleton(pipelineHandlerCollection);
                

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Pipelined API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pipelined API");
            });

            app.UseMvc();
        }
    }
}
