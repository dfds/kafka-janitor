using KafkaJanitor.App.Domain;
using KafkaJanitor.App.Infrastructure.Messaging;
using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.App.Configurations;

public static class AOP
{
    public static void ConfigureAspects(this WebApplicationBuilder builder)
    {
        builder.Services.RewireWithAspects(options =>
        {
            options.Register<TransactionalAttribute, TransactionalAspect>();
            options.Register<OutboxedAttribute, OutboxEnqueuerAspect>();
        });
    }
}