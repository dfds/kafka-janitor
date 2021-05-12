using KafkaJanitor.RestApi.Features;
using KafkaJanitor.RestApi.Features.Vault;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaJanitor.RestApi.Enablers.PrometheusHealthCheck
{
    public static class HealthChecks
    {
        public static IHealthChecksBuilder AddOurChecks(this IHealthChecksBuilder healthChecksBuilder, bool enableTikaHealthCheck)
        {
            healthChecksBuilder
                .AddCheck("assembly", () => HealthCheckResult.Healthy())
                .AddCheck<VaultHealthCheck>("Vault");

            if (enableTikaHealthCheck)
            {
                healthChecksBuilder.AddCheck<TikaHealthCheck>("Tika");
            }

            return healthChecksBuilder;
        }
    }
}