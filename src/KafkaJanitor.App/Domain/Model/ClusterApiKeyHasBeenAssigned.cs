namespace KafkaJanitor.App.Domain.Model;

public class ClusterApiKeyHasBeenAssigned : IDomainEvent
{
    public string? ServiceAccountId { get; set; }
    public string? ClusterApiKeyId { get; set; }
}