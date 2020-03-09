using Microsoft.Extensions.DependencyInjection;

namespace KafkaJanitor.RestApi.Enablers.PrometheusHealthCheck
{
    public static class DependencyInjection
    {
        public static IHealthChecksBuilder ConfigureHealthChecks(
            this IServiceCollection services
        )
        {
            var healthChecksBuilder = services
                .AddHealthChecks()
                .AddOurChecks();
            
            return healthChecksBuilder;
        }
    }
}