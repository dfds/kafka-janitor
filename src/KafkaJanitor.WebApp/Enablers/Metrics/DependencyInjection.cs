using Microsoft.Extensions.DependencyInjection;

namespace KafkaJanitor.WebApp.Enablers.Metrics
{
    public static class DependencyInjection
    {
        public static void AddMetrics(this IServiceCollection services)
        {
            services.AddHostedService<MetricHostedService>();
        }
    }
}