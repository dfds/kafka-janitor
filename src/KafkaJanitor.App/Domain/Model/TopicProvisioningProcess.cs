using KafkaJanitor.App.Domain.Events;

namespace KafkaJanitor.App.Domain.Model;

public class TopicProvisioningProcess : AggregateRoot<TopicProvisionProcessId>
{
    public TopicProvisioningProcess(TopicProvisionProcessId id, TopicName requestedTopic, CapabilityRootId capabilityRootId, ClusterId clusterId, 
        TopicPartition partitions, TopicRetention retention, bool isServiceAccountCreated, bool isServiceAccountGrantedAccessToCluster, 
        bool isTopicProvisioned, bool isApiKeyStoredInVault, bool isCompleted) : base(id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (requestedTopic == null)
        {
            throw new ArgumentNullException(nameof(requestedTopic));
        }

        if (capabilityRootId == null)
        {
            throw new ArgumentNullException(nameof(capabilityRootId));
        }

        if (clusterId == null)
        {
            throw new ArgumentNullException(nameof(clusterId));
        }

        if (partitions == null)
        {
            throw new ArgumentNullException(nameof(partitions));
        }

        if (retention == null)
        {
            throw new ArgumentNullException(nameof(retention));
        }

        RequestedTopic = requestedTopic;
        CapabilityRootId = capabilityRootId;
        ClusterId = clusterId;
        Partitions = partitions;
        Retention = retention;
        IsServiceAccountCreated = isServiceAccountCreated;
        IsServiceAccountGrantedAccessToCluster = isServiceAccountGrantedAccessToCluster;
        IsTopicProvisioned = isTopicProvisioned;
        IsApiKeyStoredInVault = isApiKeyStoredInVault;
        IsCompleted = isCompleted;
    }

    public TopicName RequestedTopic { get; private set; }
    public ClusterId ClusterId { get; private set; }
    public TopicPartition Partitions { get; private set; }
    public TopicRetention Retention { get; private set; }
    
    public CapabilityRootId CapabilityRootId { get; private set; }

    public bool IsServiceAccountCreated { get; private set; }
    public bool IsServiceAccountGrantedAccessToCluster { get; private set; }
    public bool IsTopicProvisioned { get; private set; }
    public bool IsApiKeyStoredInVault { get; private set; }
    public bool IsCompleted { get; private set; }

    public void RegisterThatCapabilityHasServiceAccount()
    {
        IsServiceAccountCreated = true;

        Raise(new CapabilityHasServiceAccount
        {
            ProcessId = Id.ToString(),
        });
    }

    public void RegisterThatServiceAccountHasAccessToCluster()
    {
        IsServiceAccountGrantedAccessToCluster = true;
        CheckProcessStatus();
    }

    public void RegisterThatApiKeyIsStoredInVault()
    {
        IsApiKeyStoredInVault = true;
        CheckProcessStatus();
    }

    public void RegisterThatTopicIsProvisioned()
    {
        IsTopicProvisioned = true;
        CheckProcessStatus();
    }

    private void CheckProcessStatus()
    {
        if (IsCompleted)
        {
            return; // already completed - nothing to do!
        }

        if (IsServiceAccountCreated && IsServiceAccountGrantedAccessToCluster && IsApiKeyStoredInVault && IsApiKeyStoredInVault && IsTopicProvisioned)
        {
            IsCompleted = true;
            Raise(new TopicProvisioningProcessHasCompleted
            {
                ProcessId = Id.ToString()
            });
        }
    }

    public static TopicProvisioningProcess Start(TopicName requestedTopic, ClusterId clusterId, TopicPartition partitions, TopicRetention retention)
    {
        var instance = new TopicProvisioningProcess(
            id: TopicProvisionProcessId.New(),
            requestedTopic: requestedTopic,
            capabilityRootId: requestedTopic.CapabilityRootId,
            clusterId: clusterId,
            partitions: partitions,
            retention: retention,
            isServiceAccountCreated: false,
            isServiceAccountGrantedAccessToCluster: false,
            isTopicProvisioned: false,
            isApiKeyStoredInVault: false,
            isCompleted: false
        );

        instance.Raise(new TopicProvisioningProcessHasStarted
        {
            ProcessId = instance.Id.ToString()
        });

        return instance;
    }
}