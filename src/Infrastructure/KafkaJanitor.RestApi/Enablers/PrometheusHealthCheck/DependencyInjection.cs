using KafkaJanitor.RestApi.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KafkaJanitor.RestApi.Enablers.PrometheusHealthCheck
{
    public static class DependencyInjection
    {
        public static IHealthChecksBuilder ConfigureHealthChecks(
            this IServiceCollection services
        )
        {
     
            
       //     services.AddSingleton<IHealthCheck, TikaHealthCheck>();
            
            var healthChecksBuilder = services
                .AddHealthChecks()
                .AddOurChecks();
            
            return healthChecksBuilder;
        }
    }
}