using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class TopicProvisioningProcessHasStarted : IDomainEvent
{
    public string? ProcessId { get; set; }
}