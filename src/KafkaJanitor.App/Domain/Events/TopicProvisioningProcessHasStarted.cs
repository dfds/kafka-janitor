using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Events;

public class TopicProvisioningProcessHasStarted : IDomainEvent
{
    public string? ProcessId { get; set; }
}


/*
    - create service account for capability
    - create cluster access definition for service account and cluster
    - apply acl entries specified within cluster access definition
    - create api keys for service account to specific cluster
    - store api keys in vault
 */