using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class ServiceAccountHasBeenDefined : IDomainEvent
{
    public string? ServiceAccountId { get; set; }
}