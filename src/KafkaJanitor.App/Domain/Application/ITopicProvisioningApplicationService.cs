using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Application;

public interface ITopicProvisioningApplicationService
{
    Task<TopicProvisionProcessId> StartProvisioningProcess(TopicName requestedTopic, ClusterId clusterId, TopicPartition partitions, TopicRetention retention);
    Task CreateTopicFrom(TopicProvisionProcessId processId);
    Task CreateServiceAccount(TopicProvisionProcessId processId);
    Task RegisterServiceAccountForProcess(ServiceAccountId serviceAccountId);
    Task CreateMissingACLEntry(ServiceAccountId serviceAccountId);
    Task RegisterServiceAccountHasAccess(ServiceAccountId serviceAccountId);
    Task AssignNextMissingApiKeyForServiceAccount(ServiceAccountId serviceAccountId);
    Task StoreApiKeyInVault(ServiceAccountId serviceAccountId, ClusterApiKeyId apiKeyId);
    Task UpdateProcessWhenAllApiKeysAreStoredInVault(ServiceAccountId serviceAccountId);
}