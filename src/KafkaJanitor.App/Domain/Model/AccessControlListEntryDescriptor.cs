using System.ComponentModel;

namespace KafkaJanitor.App.Domain.Model;

public class AccessControlListEntryDescriptor : ValueObject
{
    public AccessControlListEntryDescriptor(ACLEntryResourceType resourceType, string resourceName, ACLEntryPatternType patternType, 
        ACLEntryOperationType operationType, ACLEntryPermissionType permissionType)
    {
        if (!Enum.IsDefined(typeof(ACLEntryResourceType), resourceType))
        {
            throw new InvalidEnumArgumentException(nameof(resourceType), (int) resourceType, typeof(ACLEntryResourceType));
        }

        if (string.IsNullOrWhiteSpace(resourceName))
        {
            throw new ArgumentException("Value for resource name cannot be null or whitespace.", nameof(resourceName));
        }

        if (!Enum.IsDefined(typeof(ACLEntryPatternType), patternType))
        {
            throw new InvalidEnumArgumentException(nameof(patternType), (int) patternType, typeof(ACLEntryPatternType));
        }

        if (!Enum.IsDefined(typeof(ACLEntryOperationType), operationType))
        {
            throw new InvalidEnumArgumentException(nameof(operationType), (int) operationType, typeof(ACLEntryOperationType));
        }

        if (!Enum.IsDefined(typeof(ACLEntryPermissionType), permissionType))
        {
            throw new InvalidEnumArgumentException(nameof(permissionType), (int) permissionType, typeof(ACLEntryPermissionType));
        }

        ResourceType = resourceType;
        ResourceName = resourceName;
        PatternType = patternType;
        OperationType = operationType;
        PermissionType = permissionType;
    }

    public ACLEntryResourceType ResourceType { get; }
    public string ResourceName { get; }
    public ACLEntryPatternType PatternType { get; }
    public ACLEntryOperationType OperationType { get; }
    public ACLEntryPermissionType PermissionType { get; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ResourceType;
        yield return ResourceName;
        yield return PatternType;
        yield return OperationType;
        yield return PermissionType;
    }

    public override string ToString()
        => string.Join(" | ",
            ResourceType.ToString("G"),
            ResourceName,
            PatternType.ToString("G"),
            OperationType.ToString("G"),
            PermissionType.ToString("G")
        );
}