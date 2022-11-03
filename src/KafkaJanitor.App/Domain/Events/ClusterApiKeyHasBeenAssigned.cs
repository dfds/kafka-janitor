using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class ClusterApiKeyHasBeenAssigned : IDomainEvent
{
    public string? ServiceAccountId { get; set; }
    public string? ClusterApiKeyId { get; set; }
}