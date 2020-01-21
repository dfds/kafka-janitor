using System;
using KafkaJanitor.WebApp.Infrastructure.Http;
using KafkaJanitor.WebApp.Infrastructure.Messaging;
using KafkaJanitor.WebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tika.Client;

namespace KafkaJanitor.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TikaOptions>(Configuration);
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<ForwardedHeaderBasePath>();

            services.AddTransient<ITopicRepository, TopicCcloudRepository>();
            services.AddSingleton<KafkaConfiguration>();

            services.AddHttpClient<ITikaClient, TikaClient>(cfg =>
            {
                var baseUrl = Configuration["TIKA_API_ENDPOINT"];
                if (baseUrl != null)
                {
                    cfg.BaseAddress = new Uri(baseUrl);
                }
            });
            services.AddTransient<ITikaClient, TikaClient>();

            services.AddTransient<MessageHandler>();
            services.AddHostedService<TopicSubscriber>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeadersAsBasePath();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
