using KafkaJanitor.App.Domain.Events;
using static KafkaJanitor.App.Domain.Model.ACLEntryOperationType;
using static KafkaJanitor.App.Domain.Model.ACLEntryPermissionType;

namespace KafkaJanitor.App.Domain.Model;

public class ServiceAccount : AggregateRoot<ServiceAccountId>
{
    private readonly List<ClusterApiKey> _clusterApiKeys = null!;
    private readonly List<AccessControlListEntry> _accessControlList = null!;

    private ServiceAccount() { }

    public ServiceAccount(ServiceAccountId id, CapabilityRootId capabilityRootId, IEnumerable<AccessControlListEntry> accessControlList, IEnumerable<ClusterApiKey> clusterApiKeys) : base(id)
    {
        if (capabilityRootId == null)
        {
            throw new ArgumentNullException(nameof(capabilityRootId), "Capability root id value cannot be null.");
        }

        CapabilityRootId = capabilityRootId;
        _accessControlList = new List<AccessControlListEntry>(accessControlList);
        _clusterApiKeys = new List<ClusterApiKey>(clusterApiKeys);
    }

    public CapabilityRootId CapabilityRootId { get; private set; } = null!;

    [Obsolete("will become its own aggregate")]
    public IEnumerable<AccessControlListEntry> AccessControlList => _accessControlList;

    [Obsolete("will become its own aggregate")]
    public IEnumerable<ClusterApiKey> ClusterApiKeys => _clusterApiKeys;

    public bool HasApiKeyFor(ClusterId cluster) 
        => _clusterApiKeys.Any(x => x.ClusterId == cluster);

    public void AssignClusterApiKey(ClusterId clusterId, string userName, string password)
    {
        // TODO [jandr@2022-10-27]: null/empty checks on parameter values or convert back to injecting the descriptor instead

        var apiKey = ClusterApiKey.Create(userName, password, clusterId);
        _clusterApiKeys.Add(apiKey);

        Raise(new ClusterApiKeyHasBeenAssigned
        {
            ServiceAccountId = Id.ToString(),
            ClusterApiKeyId = apiKey.Id.ToString(),
        });
    }

    public bool FindApiKeyBy(ClusterApiKeyId id, out ClusterApiKey apiKey)
    {
        var key = _clusterApiKeys.SingleOrDefault(x => x.Id == id);
        if (key is null)
        {
            apiKey = null!;
            return false;
        }

        apiKey = key;
        return true;
    }

    public bool FindApiKeyBy(ClusterId clusterId, out ClusterApiKey apiKey)
    {
        var key = _clusterApiKeys.SingleOrDefault(x => x.ClusterId == clusterId);
        if (key is null)
        {
            apiKey = null!;
            return false;
        }

        apiKey = key;
        return true;
    }

    public void RegisterApiKeyAsStoredInVault(ClusterApiKeyId apiKeyId)
    {
        if (!FindApiKeyBy(apiKeyId, out var apiKey))
        {
            throw new Exception($"Unknown api key for {apiKeyId}");
        }

        apiKey.MarkAsStoredInVault();

        Raise(new ApiKeyHasBeenStoredInVault
        {
            ServiceAccountId = Id.ToString(),
            ClusterApiKeyId = apiKey.Id.ToString()
        });
    }

    public static ServiceAccount DefineNew(ServiceAccountId id, CapabilityRootId capabilityRootId)
    {
        var acl = CreateDefaultAccessControlList(capabilityRootId);

        var serviceAccount = new ServiceAccount(
            id: id,
            capabilityRootId: capabilityRootId,
            accessControlList: acl,
            clusterApiKeys: Enumerable.Empty<ClusterApiKey>()
        );

        serviceAccount.Raise(new ServiceAccountHasBeenDefined
        {
            ServiceAccountId = serviceAccount.Id.ToString()
        });

        return serviceAccount;
    }

    [Obsolete]
    public static AccessControlListEntry[] CreateDefaultAccessControlList(CapabilityRootId capabilityRootId)
    {
        return new[]
        {
            // deny create operations on all resource types
            AccessControlListEntry.CreateForTopicPrefix("'*'", Create, Deny),

            // for all private topics
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Read, Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Write, Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Create, Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Describe, Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), DescribeConfigs, Allow),

            // for all public topics
            AccessControlListEntry.CreateForTopicPrefix("pub.", Read, Allow),

            // for own public topics
            AccessControlListEntry.CreateForTopicPrefix($"pub.{capabilityRootId}.", Write, Allow),
            AccessControlListEntry.CreateForTopicPrefix($"pub.{capabilityRootId}.", Create, Allow),

            // for all connect groups
            AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", Read, Allow),
            AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", Write, Allow),
            AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", Create, Allow),

            // for all capability groups
            AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), Read, Allow),
            AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), Write, Allow),
            AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), Create, Allow),

            // for cluster
            AccessControlListEntry.Create(
                resourceType: ACLEntryResourceType.Cluster,
                resourceName: "kafka-cluster",
                patternType: ACLEntryPatternType.Literal,
                operationType: Alter,
                permissionType: Deny
            ),

            AccessControlListEntry.Create(
                resourceType: ACLEntryResourceType.Cluster,
                resourceName: "kafka-cluster",
                patternType: ACLEntryPatternType.Literal,
                operationType: AlterConfigs,
                permissionType: Deny
            ),

            AccessControlListEntry.Create(
                resourceType: ACLEntryResourceType.Cluster,
                resourceName: "kafka-cluster",
                patternType: ACLEntryPatternType.Literal,
                operationType: ClusterAction,
                permissionType: Deny
            ),
        };
    }
}