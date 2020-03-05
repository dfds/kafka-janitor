using KafkaJanitor.RestApi.Features;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaJanitor.RestApi.Enablers.PrometheusHealthCheck
{
    public static class HealthChecks
    {
        public static IHealthChecksBuilder AddOurChecks(this IHealthChecksBuilder healthChecksBuilder)
        {
            healthChecksBuilder
                .AddCheck("assembly", () => HealthCheckResult.Healthy())
                .AddCheck<TikaHealthCheck>("Tika");
            return healthChecksBuilder;
        }
    }
}