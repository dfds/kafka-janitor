using KafkaJanitor.App.Domain.Events;

namespace KafkaJanitor.App.Domain.Model;

public class TopicProvisioningProcess : AggregateRoot<TopicProvisionProcessId>
{
    public TopicProvisioningProcess(TopicProvisionProcessId id, TopicName requestedTopic, ClusterId clusterId, TopicPartition partitions, 
        TopicRetention retention, bool isServiceAccountCreated, bool isServiceAccountGrantedAccess, bool isTopicProvisioned, bool isApiKeysCreated,
        bool areAllApiKeysStoredInVault, bool isCompleted) : base(id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (requestedTopic == null)
        {
            throw new ArgumentNullException(nameof(requestedTopic));
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
        ClusterId = clusterId;
        Partitions = partitions;
        Retention = retention;
        IsServiceAccountCreated = isServiceAccountCreated;
        IsServiceAccountGrantedAccess = isServiceAccountGrantedAccess;
        IsTopicProvisioned = isTopicProvisioned;
        IsApiKeysCreated = isApiKeysCreated;
        AreAllApiKeysStoredInVault = areAllApiKeysStoredInVault;
        IsCompleted = isCompleted;
    }

    public TopicName RequestedTopic { get; private set; }
    public ClusterId ClusterId { get; private set; }
    public TopicPartition Partitions { get; private set; }
    public TopicRetention Retention { get; private set; }
    
    public CapabilityRootId CapabilityRootId => RequestedTopic.CapabilityRootId;

    public bool IsServiceAccountCreated { get; private set; }
    public bool IsServiceAccountGrantedAccess { get; private set; }
    public bool IsTopicProvisioned { get; private set; }
    public bool IsApiKeysCreated { get; private set; }
    public bool AreAllApiKeysStoredInVault { get; private set; }
    public bool IsCompleted { get; private set; }

    public void RegisterThatCapabilityHasServiceAccount()
    {
        IsServiceAccountCreated = true;
        CheckProcessStatus();
    }

    public void RegisterThatServiceAccountHasAccess()
    {
        IsServiceAccountGrantedAccess = true;
        CheckProcessStatus();
    }

    public void RegisterThatServiceAccountHasAllApiKeys()
    {
        IsApiKeysCreated = true;
        CheckProcessStatus();
    }

    public void RegisterThatAllApiKeysAreStoredInVault()
    {
        AreAllApiKeysStoredInVault = true;
        CheckProcessStatus();
    }

    public void RegisterTopicAsProvisioned()
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

        if (IsServiceAccountCreated && IsServiceAccountGrantedAccess && IsApiKeysCreated && AreAllApiKeysStoredInVault && IsTopicProvisioned)
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
            clusterId: clusterId,
            partitions: partitions,
            retention: retention,
            isServiceAccountCreated: false,
            isServiceAccountGrantedAccess: false,
            isTopicProvisioned: false,
            isApiKeysCreated: false,
            areAllApiKeysStoredInVault: false,
            isCompleted: false
        );

        instance.Raise(new TopicProvisioningProcessHasStarted
        {
            ProcessId = instance.Id.ToString()
        });

        return instance;
    }
}