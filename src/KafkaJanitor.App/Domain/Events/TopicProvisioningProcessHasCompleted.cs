using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class TopicProvisioningProcessHasCompleted : IDomainEvent
{
    public string? ProcessId { get; set; }
}