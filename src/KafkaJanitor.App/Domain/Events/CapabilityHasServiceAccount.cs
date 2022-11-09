using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class CapabilityHasServiceAccount : IDomainEvent
{
    public string? ProcessId { get; set; }
}