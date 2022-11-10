using Dafda.Configuration;
using KafkaJanitor.App.Configurations.DomainEventAutoRegistrations;
using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.App.Configurations;

public static class DomainEvents
{
    public static void ConfigureDomainEvents(this WebApplicationBuilder builder)
    {
        // get all auto-register registrators
        var registrators = AutoDomainEventRegistrationBase.DiscoverAndCreateAllRegistrators(builder.Configuration);

        // auto register consumers
        AutoRegisterConsumers(builder, registrators);
        AutoRegisterProducers(builder, registrators);
    }

    private static void AutoRegisterConsumers(WebApplicationBuilder builder, IEnumerable<AutoDomainEventRegistrationBase> registrators)
    {
        foreach (var registrator in registrators)
        {
            foreach (var consumerRegistration in registrator.GetConsumerRegistrations())
            {
                builder.Services.AddConsumer(options =>
                {
                    options.WithEnvironmentStyle("DEFAULT_KAFKA");
                    options.WithGroupId(consumerRegistration.GroupId);
                    options.WithConfigurationSource(builder.Configuration);

                    consumerRegistration.ApplyTo(options);
                });
            }
        }
    }

    private static void AutoRegisterProducers(WebApplicationBuilder builder, IEnumerable<AutoDomainEventRegistrationBase> registrators)
    {
        // Register producers indirectly by setting up an outbox registration for them.
        // Actual producing occurs out-of-band (out-of-process) with the usage of "Dafda Gendis".

        builder.Services.AddOutbox(options =>
        {
            // auto register producers
            foreach (var registrator in registrators)
            {
                foreach (var producerRegistration in registrator.GetProducerRegistrations())
                {
                    producerRegistration.ApplyTo(options);
                }
            }

            options.WithOutboxEntryRepository<OutboxEntryRepository>();
        });
    }
}