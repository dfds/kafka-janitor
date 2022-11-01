namespace KafkaJanitor.App.Domain.Model;

public class ApiKeyHasBeenStoredInVault : IDomainEvent
{
    public string? ServiceAccountId { get; set; }
    public string? ClusterApiKeyId { get; set; }
}