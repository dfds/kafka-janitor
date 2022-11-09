using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Application;

public interface ITopicProvisioningApplicationService
{
    Task<TopicProvisionProcessId> StartProvisioningProcess(TopicName requestedTopic, ClusterId clusterId, TopicPartition partitions, TopicRetention retention);
    Task ProvisionTopicFrom(TopicProvisionProcessId processId);
    Task EnsureCapabilityHasServiceAccount(TopicProvisionProcessId processId);
    Task RegisterNewServiceAccountIsDefined(ServiceAccountId serviceAccountId);
    Task ApplyNextMissingACLEntry(ClusterAccessDefinitionId clusterAccessDefinitionId);
    Task RegisterServiceAccountHasAccess(ClusterAccessDefinitionId clusterAccessDefinitionId);
    Task EnsureServiceAccountHasApiKey(TopicProvisionProcessId processId);
    Task StoreApiKeyInVault(ServiceAccountId serviceAccountId, ClusterApiKeyId apiKeyId);
    Task UpdateProcessWhenApiKeyIsStoredInVault(ServiceAccountId serviceAccountId);
    Task EnsureClusterAccess(TopicProvisionProcessId processId);
}