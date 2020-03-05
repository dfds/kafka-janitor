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
            return services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
        }
    }
}