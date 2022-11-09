using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.TestDoubles;

public class StubConfluentGateway : IConfluentGateway
{
    private readonly IEnumerable<Topic> _topics;

    private StubConfluentGateway(IEnumerable<Topic> topics) 
        => _topics = topics;

    public Task<IEnumerable<Topic>> GetAllBy(ClusterId clusterId)
        => Task.FromResult(_topics);

    public Task<bool> Exists(string topicName)
    {
        throw new System.NotImplementedException();
    }

    public Task CreateTopic(ClusterId clusterId, TopicName topic, TopicPartition partition, TopicRetention retention, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task<ServiceAccountId> CreateServiceAccount(string name, string description, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CreateACLEntry(ClusterId clusterId, ServiceAccountId serviceAccountId,
        AccessControlListEntryDescriptor entry,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ClusterApiKeyDescriptor> CreateApiKey(ClusterId clusterId, ServiceAccountId serviceAccountId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public static StubConfluentGateway AsEmpty() 
        => new StubConfluentGateway(Enumerable.Empty<Topic>());

    public static StubConfluentGateway Containing(params Topic[] topics) 
        => new StubConfluentGateway(topics);
}

public class FakeConfluentGateway : IConfluentGateway
{
    private List<Topic> _topics = new List<Topic>();

    public List<Topic> Topics => _topics;

    public Task<IEnumerable<Topic>> GetAllBy(ClusterId clusterId) 
        => Task.FromResult(_topics.AsEnumerable());

    public Task<bool> Exists(string topicName)
    {
        var result = _topics.Any(x => x.Name.Equals(topicName, StringComparison.InvariantCultureIgnoreCase));
        return Task.FromResult(result);
    }

    public Task CreateTopic(ClusterId clusterId, TopicName topic, TopicPartition partition, TopicRetention retention, CancellationToken cancellationToken) 
        => Task.CompletedTask;

    public Task<ServiceAccountId> CreateServiceAccount(string name, string description, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CreateACLEntry(ClusterId clusterId, ServiceAccountId serviceAccountId,
        AccessControlListEntryDescriptor entry,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ClusterApiKeyDescriptor> CreateApiKey(ClusterId clusterId, ServiceAccountId serviceAccountId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
