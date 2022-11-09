using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class ACLEntryHasBeenAssigned : IDomainEvent
{
    public string? ClusterAccessDefinitionId { get; set; }
    public string? AccessControlListEntryId { get; set; }
}