using Microsoft.Extensions.DependencyInjection;

namespace KafkaJanitor.RestApi.Enablers.Metrics
{
    public static class DependencyInjection
    {
        public static void AddMetrics(this IServiceCollection services, bool startHostedService)
        {
            if(startHostedService == false) {return;}
            
            services.AddHostedService<MetricHostedService>();
        }
    }
}