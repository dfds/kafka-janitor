using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class NewClusterAccessDefinitionHasBeenDefined : IDomainEvent
{
    public string? ClusterAccessDefinitionId { get; set; }
    public string? ClusterId { get; set; }
    public string? ServiceAccountId { get; set; }
}