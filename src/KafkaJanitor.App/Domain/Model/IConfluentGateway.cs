namespace KafkaJanitor.App.Domain.Model;

public interface IConfluentGateway
{
    [Obsolete]
    Task<IEnumerable<Topic>> GetAllBy(ClusterId clusterId);
    
    [Obsolete]
    Task<bool> Exists(string topicName);

    Task CreateTopic(ClusterId clusterId, TopicName topic, TopicPartition partition, TopicRetention retention, CancellationToken cancellationToken);
    Task<ServiceAccountId> CreateServiceAccount(string name, string description, CancellationToken cancellationToken);
    Task CreateACLEntry(ServiceAccountId serviceAccountId, AccessControlListEntryDescriptor entry, CancellationToken cancellationToken);
    Task<ClusterApiKeyDescriptor> CreateApiKey(ClusterId clusterId, ServiceAccountId serviceAccountId, CancellationToken cancellationToken);
}