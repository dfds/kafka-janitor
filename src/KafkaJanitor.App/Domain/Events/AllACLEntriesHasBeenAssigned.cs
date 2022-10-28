using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class AllACLEntriesHasBeenAssigned : IDomainEvent
{
    public string? ServiceAccountId { get; set; }
}