using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class TopicProvisioningProcessBuilder
{
    private TopicProvisionProcessId _id;
    private TopicName _requestedTopic;
    private CapabilityRootId _capabilityRootId;
    private ClusterId _clusterId;
    private TopicPartition _partitions;
    private TopicRetention _retention;
    private bool _isServiceAccountCreated;
    private bool _isServiceAccountGrantedAccess;
    private bool _isTopicProvisioned;
    private bool _isApiKeysCreated;
    private bool _areAllApiKeysStoredInVault;
    private bool _isCompleted;

    public TopicProvisioningProcessBuilder()
    {
        _id = TopicProvisionProcessId.New();
        _requestedTopic = A.TopicName.Build();
        _capabilityRootId = CapabilityRootId.Parse("foo");
        _clusterId = ClusterId.None;
        _partitions = TopicPartition.From(1);
        _retention = TopicRetention.Infinite;
        _isServiceAccountCreated = true;
        _isServiceAccountGrantedAccess = true;
        _isTopicProvisioned = true;
        _isApiKeysCreated = true;
        _areAllApiKeysStoredInVault = true;
        _isCompleted = true;
    }

    public TopicProvisioningProcessBuilder WithClusterId(ClusterId clusterId)
    {
        _clusterId = clusterId;
        return this;
    }

    public TopicProvisioningProcessBuilder WithCapabilityRootId(CapabilityRootId capabilityRootId)
    {
        _capabilityRootId = capabilityRootId;
        return this;
    }

    public TopicProvisioningProcessBuilder WithIsCompleted(bool isCompleted)
    {
        _isCompleted = isCompleted;
        return this;
    }

    public TopicProvisioningProcess Build()
    {
        return new TopicProvisioningProcess(
            id: _id,
            requestedTopic: _requestedTopic,
            capabilityRootId: _capabilityRootId,
            clusterId: _clusterId,
            partitions: _partitions,
            retention: _retention,
            isServiceAccountCreated: _isServiceAccountCreated,
            isServiceAccountGrantedAccess: _isServiceAccountGrantedAccess,
            isTopicProvisioned: _isTopicProvisioned,
            isApiKeysCreated: _isApiKeysCreated,
            areAllApiKeysStoredInVault: _areAllApiKeysStoredInVault,
            isCompleted: _isCompleted
        );
    }
}