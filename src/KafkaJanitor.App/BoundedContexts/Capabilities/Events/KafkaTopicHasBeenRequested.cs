using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.BoundedContexts.Capabilities.Events;

public class KafkaTopicHasBeenRequested : IDomainEvent
{
    public string? CapabilityRootId { get; set; }
    public string? ClusterId { get; set; }
    public string? TopicName { get; set; }
    public int? Partitions { get; set; }
    public int? Retention { get; set; }
}