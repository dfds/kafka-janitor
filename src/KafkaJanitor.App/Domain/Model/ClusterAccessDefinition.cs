using KafkaJanitor.App.Domain.Events;

namespace KafkaJanitor.App.Domain.Model;

public class ClusterAccessDefinition : AggregateRoot<ClusterAccessDefinitionId>
{
    private readonly List<AccessControlListEntry> _accessControlList = null!;
    private readonly ClusterId _clusterId = null!;
    private readonly ServiceAccountId _serviceAccountId = null!;

    private ClusterAccessDefinition() { }

    public ClusterAccessDefinition(ClusterAccessDefinitionId id, ClusterId clusterId, ServiceAccountId serviceAccountId, IEnumerable<AccessControlListEntry> accessControlList) : base(id)
    {
        _clusterId = clusterId;
        _serviceAccountId = serviceAccountId;
        _accessControlList = new List<AccessControlListEntry>(accessControlList);
    }

    public ClusterId ClusterId => _clusterId;
    public ServiceAccountId ServiceAccountId => _serviceAccountId;
    public IEnumerable<AccessControlListEntry> AccessControlList => _accessControlList;

    public bool IsFullyApplied => _accessControlList.All(x => x.IsApplied);

    public AccessControlListEntry? FindNextUnAppliedAccessControlListEntry()
    {
        return AccessControlList.FirstOrDefault(x => !x.IsApplied);
    }

    public void RegisterAccessControlListEntryAsApplied(AccessControlListEntryId entryId)
    {
        var entry = AccessControlList.SingleOrDefault(x => x.Id == entryId);
        if (entry is null)
        {
            throw new Exception($"Access control list entry \"{entryId}\" does not belong to cluster access definition \"{Id}\".");
        }

        if (entry.IsApplied)
        {
            return;
        }

        entry.RegisterAsApplied();

        Raise(new ACLEntryHasBeenAssigned
        {
            ClusterAccessDefinitionId = Id.ToString(),
            AccessControlListEntryId = entry.Id.ToString()
        });

        if (IsFullyApplied)
        {
            Raise(new AllACLEntriesHasBeenAssigned
            {
                ClusterAccessDefinitionId = Id.ToString()
            });
        }
    }

    public static ClusterAccessDefinition DefineNew(ServiceAccountId serviceAccountId, ClusterId clusterId, IEnumerable<AccessControlListEntry> acl)
    {
        var instance = new ClusterAccessDefinition(
            id: ClusterAccessDefinitionId.New(),
            clusterId: clusterId,
            serviceAccountId: serviceAccountId,
            accessControlList: acl.ToArray()
        );

        instance.Raise(new NewClusterAccessDefinitionHasBeenDefined
        {
            ClusterAccessDefinitionId = instance.Id.ToString(),
            ClusterId = clusterId.ToString(),
            ServiceAccountId = serviceAccountId.ToString()
        });

        return instance;
    }
}