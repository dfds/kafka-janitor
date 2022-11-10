using KafkaJanitor.App.BoundedContexts.Capabilities.Events;
using KafkaJanitor.App.Domain.Policies;
using KafkaJanitor.App.Infrastructure.Messaging;

namespace KafkaJanitor.App.Configurations.DomainEventAutoRegistrations;

public class CapabilitiesContextRegistrations : AutoDomainEventRegistrationBase
{
    public CapabilitiesContextRegistrations(IConfiguration configuration) : base(configuration)
    {

    }

    private string KafkaTopicTopicName => GetTopicNameBy("TOPIC_CAPABILITIES_KAFKATOPIC");

    public override IEnumerable<ConsumerRegistration> GetConsumerRegistrations()
    {
        yield return new ConsumerRegistration("build.selfservice.kafka-janitor", options =>
        {
            options
                .ForTopic(KafkaTopicTopicName)
                .RegisterMessageHandler<KafkaTopicHasBeenRequested, WhenKafkaTopicHasBeenRequestedThenStartProvisioningProcess>("kafkatopic-has-been-requested")
                .IgnoreTheRest();
        });
    }
}

public class TopicProvisioningRegistrations : AutoDomainEventRegistrationBase
{
    public TopicProvisioningRegistrations(IConfiguration configuration) : base(configuration)
    {

    }

    private string KafkaTopicTopicName => GetTopicNameBy("TOPIC_CAPABILITIES_KAFKATOPIC");

    public override IEnumerable<ConsumerRegistration> GetConsumerRegistrations()
    {
        yield return new ConsumerRegistration("build.selfservice.kafka-janitor", options =>
        {
            options
                .ForTopic(KafkaTopicTopicName)
                .RegisterMessageHandler<KafkaTopicHasBeenRequested, WhenKafkaTopicHasBeenRequestedThenStartProvisioningProcess>("kafkatopic-has-been-requested")
                .IgnoreTheRest();
        });
    }
}
