namespace KafkaJanitor.App.Domain.Model;

public interface IConfluentGateway
{
    Task<IEnumerable<Topic>> GetAllBy(ClusterId clusterId);
    Task<bool> Exists(string topicName);
    Task CreateTopic(ClusterId clusterId, TopicName topic, TopicPartition partition, TopicRetention retention);
    Task<ServiceAccountId> CreateServiceAccount(string name, string description);
    Task CreateACLEntry(ServiceAccountId serviceAccountId, AccessControlListEntryDescriptor entry);
    Task<ClusterApiKeyDescriptor> CreateApiKey(ClusterId clusterId, ServiceAccountId serviceAccountId);
}