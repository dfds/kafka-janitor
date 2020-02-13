using Microsoft.Extensions.DependencyInjection;

namespace KafkaJanitor.RestApi.Enablers.Metrics
{
    public static class DependencyInjection
    {
        public static void AddMetrics(this IServiceCollection services)
        {
            services.AddHostedService<MetricHostedService>();
        }
    }
}