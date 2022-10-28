using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Infrastructure.ConfluentCloud;

public class ConfluentGateway : IConfluentGateway
{
    private readonly HttpClient _client;

    public ConfluentGateway(HttpClient client)
    {
        _client = client;
    }

    public Task<IEnumerable<Topic>> GetAllBy(ClusterId clusterId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(string topicName)
    {
        throw new NotImplementedException();
    }

    public Task CreateTopic(ClusterId clusterId, TopicName topic, TopicPartition partition, TopicRetention retention)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceAccountId> CreateServiceAccount(string name, string description)
    {
        throw new NotImplementedException();
    }

    public Task CreateACLEntry(ServiceAccountId serviceAccountId, AccessControlListEntryDescriptor entry)
    {
        throw new NotImplementedException();
    }

    public Task<ClusterApiKeyDescriptor> CreateApiKey(ClusterId clusterId, ServiceAccountId serviceAccountId)
    {
        throw new NotImplementedException();
    }
}