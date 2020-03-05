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
            var healthChecksBuilder = services
                .AddHealthChecks();

            foreach (var healthCheck in HealthChecks.Checks)
            {
                healthChecksBuilder.AddCheck(
                    healthCheck.Key, 
                    healthCheck.Value
                );
            }

            return healthChecksBuilder;
        }
    }
}