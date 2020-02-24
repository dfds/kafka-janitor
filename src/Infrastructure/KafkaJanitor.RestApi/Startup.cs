using System;
using KafkaJanitor.RestApi.Enablers.Metrics;
using KafkaJanitor.RestApi.Features.AccessControlLists.Infrastructure;
using KafkaJanitor.RestApi.Features.ApiKeys;
using KafkaJanitor.RestApi.Features.ServiceAccounts.Infrastructure;
using KafkaJanitor.RestApi.Features.Topics.Domain;
using KafkaJanitor.RestApi.Features.Topics.Infrastructure;
using KafkaJanitor.RestApi.Features.Vault;
using KafkaJanitor.RestApi.Features.Vault.Model;
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

            services.AddTransient<IApiKeyClient, ApiKeyClient>();

            services.AddTransient<IVault>(o =>
            {
                var vaultToUse = Configuration["KAFKAJANITOR_VAULT"];

                switch (vaultToUse)
                {
                    case "AWS_SSM":
                        return new AwsSsmParameterStoreVault();
                    case "INMEMORY":
                        return new InMemoryVault();
                    default:
                        throw new InvalidVaultConfigurationException("KAFKAJANITOR_VAULT");
                }
            });

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