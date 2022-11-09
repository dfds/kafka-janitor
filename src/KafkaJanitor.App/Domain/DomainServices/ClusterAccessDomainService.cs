using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.DomainServices;

public class ClusterAccessDomainService
{
    private readonly IServiceAccountRepository _serviceAccountRepository;
    private readonly IClusterRepository _clusterRepository;

    public ClusterAccessDomainService(IServiceAccountRepository serviceAccountRepository, IClusterRepository clusterRepository)
    {
        _serviceAccountRepository = serviceAccountRepository;
        _clusterRepository = clusterRepository;
    }

    public async Task<ClusterAccessDefinition> DefineClusterAccessBetween(ServiceAccountId serviceAccountId, ClusterId clusterId)
    {
        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        var acl = CreateDefaultAccessControlList(serviceAccount.CapabilityRootId);

        return ClusterAccessDefinition.DefineNew(serviceAccountId, clusterId, acl);
    }

    public static AccessControlListEntry[] CreateDefaultAccessControlList(CapabilityRootId capabilityRootId)
    {
        return new[]
        {
            // deny create operations on all resource types
            AccessControlListEntry.CreateForTopicPrefix("'*'", ACLEntryOperationType.Create, ACLEntryPermissionType.Deny),

            // for all private topics
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), ACLEntryOperationType.Read, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), ACLEntryOperationType.Write, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), ACLEntryOperationType.Create, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), ACLEntryOperationType.Describe, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), ACLEntryOperationType.DescribeConfigs, ACLEntryPermissionType.Allow),

            // for all public topics
            AccessControlListEntry.CreateForTopicPrefix("pub.", ACLEntryOperationType.Read, ACLEntryPermissionType.Allow),

            // for own public topics
            AccessControlListEntry.CreateForTopicPrefix($"pub.{capabilityRootId}.", ACLEntryOperationType.Write, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForTopicPrefix($"pub.{capabilityRootId}.", ACLEntryOperationType.Create, ACLEntryPermissionType.Allow),

            // for all connect groups
            AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", ACLEntryOperationType.Read, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", ACLEntryOperationType.Write, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", ACLEntryOperationType.Create, ACLEntryPermissionType.Allow),

            // for all capability groups
            AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), ACLEntryOperationType.Read, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), ACLEntryOperationType.Write, ACLEntryPermissionType.Allow),
            AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), ACLEntryOperationType.Create, ACLEntryPermissionType.Allow),

            // for cluster
            AccessControlListEntry.Create(
                resourceType: ACLEntryResourceType.Cluster,
                resourceName: "kafka-cluster",
                patternType: ACLEntryPatternType.Literal,
                operationType: ACLEntryOperationType.Alter,
                permissionType: ACLEntryPermissionType.Deny
            ),

            AccessControlListEntry.Create(
                resourceType: ACLEntryResourceType.Cluster,
                resourceName: "kafka-cluster",
                patternType: ACLEntryPatternType.Literal,
                operationType: ACLEntryOperationType.AlterConfigs,
                permissionType: ACLEntryPermissionType.Deny
            ),

            AccessControlListEntry.Create(
                resourceType: ACLEntryResourceType.Cluster,
                resourceName: "kafka-cluster",
                patternType: ACLEntryPatternType.Literal,
                operationType: ACLEntryOperationType.ClusterAction,
                permissionType: ACLEntryPermissionType.Deny
            ),
        };
    }
}