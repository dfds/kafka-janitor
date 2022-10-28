using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.ConfluentCloud;

namespace KafkaJanitor.App.Configurations;

public static class Domain
{
    public static void ConfigureDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IConfluentGateway, ConfluentGateway>();
    }
}