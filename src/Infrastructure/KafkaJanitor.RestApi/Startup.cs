using KafkaJanitor.RestApi.Enablers.Metrics;
using KafkaJanitor.RestApi.Features.AccessControlLists.Infrastructure;
using KafkaJanitor.RestApi.Features.ServiceAccounts.Infrastructure;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Tika.RestClient;

namespace KafkaJanitor.RestApi
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
            services.AddControllers();
            
            services.AddTransient<ITopicRepository, TikaTopicRepository>();

            services.AddTikaRestClient(Configuration);
            
            services.AddTransient<IAccessControlListClient, AccessControlListClient>();
            
            services.AddTransient<IServiceAccountClient, ServiceAccountClient>();

            // Enablers
            services.AddMetrics();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            app.UseHttpMetrics();
        }
    }
}