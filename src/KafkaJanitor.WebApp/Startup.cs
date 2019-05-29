using KafkaJanitor.WebApp.Infrastructure.Http;
using KafkaJanitor.WebApp.Infrastructure.Messaging;
using KafkaJanitor.WebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<ForwardedHeaderBasePath>();

            services.AddTransient<ITopicRepository, TopicRepository>();
            services.AddSingleton<KafkaConfiguration>();
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
