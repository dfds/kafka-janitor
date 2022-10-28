namespace KafkaJanitor.App.Domain.Model;

public class AccessControlListEntry : Entity<AccessControlListEntryId>
{
    private AccessControlListEntry() { }

    public AccessControlListEntry(AccessControlListEntryId id, AccessControlListEntryDescriptor descriptor, bool isApplied) : base(id)
    {
        Descriptor = descriptor;
        IsApplied = isApplied;
    }

    public AccessControlListEntryDescriptor Descriptor { get; private set; } = null!;
    public bool IsApplied { get; private set; }

    public void RegisterAsAssigned()
    {
        IsApplied = true;
    }

    public override string ToString() 
        => Descriptor.ToString();

    #region factories

    public static AccessControlListEntry Create(ACLEntryResourceType resourceType, string resourceName,
        ACLEntryPatternType patternType, ACLEntryOperationType operationType, ACLEntryPermissionType permissionType)
        => new AccessControlListEntry(
            id: AccessControlListEntryId.New(),
            descriptor: new AccessControlListEntryDescriptor(
                resourceType: resourceType,
                resourceName: resourceName,
                patternType: patternType,
                operationType: operationType,
                permissionType: permissionType
            ),
            isApplied: false
        );

    public static AccessControlListEntry CreateForTopicPrefix(string topicName, ACLEntryOperationType operationType, ACLEntryPermissionType permissionType)
        => Create(
            resourceType: ACLEntryResourceType.Topic,
            resourceName: topicName,
            patternType: ACLEntryPatternType.Prefix,
            operationType: operationType,
            permissionType: permissionType
        );

    public static AccessControlListEntry CreateForGroupPrefix(string groupName, ACLEntryOperationType operationType, ACLEntryPermissionType permissionType)
        => Create(
            resourceType: ACLEntryResourceType.Group,
            resourceName: groupName,
            patternType: ACLEntryPatternType.Prefix,
            operationType: operationType,
            permissionType: permissionType
        );

    #endregion
}