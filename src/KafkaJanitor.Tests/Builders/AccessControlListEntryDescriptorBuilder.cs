using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class AccessControlListEntryDescriptorBuilder
{
    private ACLEntryResourceType _resourceType;
    private string _resourceName;
    private ACLEntryPatternType _patternType;
    private ACLEntryOperationType _operationType;
    private ACLEntryPermissionType _permissionType;

    public AccessControlListEntryDescriptorBuilder()
    {
        _resourceType = ACLEntryResourceType.Topic;
        _resourceName = "foo";
        _patternType = ACLEntryPatternType.Prefix;
        _operationType = ACLEntryOperationType.Read;
        _permissionType = ACLEntryPermissionType.Allow;
    }

    public AccessControlListEntryDescriptorBuilder WithResourceType(ACLEntryResourceType resourceType)
    {
        _resourceType = resourceType;
        return this;
    }

    public AccessControlListEntryDescriptorBuilder WithResourceName(string resourceName)
    {
        _resourceName = resourceName;
        return this;
    }

    public AccessControlListEntryDescriptorBuilder WithPatternType(ACLEntryPatternType patternType)
    {
        _patternType = patternType;
        return this;
    }

    public AccessControlListEntryDescriptorBuilder WithOperationType(ACLEntryOperationType operationType)
    {
        _operationType = operationType;
        return this;
    }

    public AccessControlListEntryDescriptorBuilder WithPermissionType(ACLEntryPermissionType permissionType)
    {
        _permissionType = permissionType;
        return this;
    }

    public AccessControlListEntryDescriptor Build()
    {
        return new AccessControlListEntryDescriptor(
            resourceType: _resourceType,
            resourceName: _resourceName,
            patternType: _patternType,
            operationType: _operationType,
            permissionType: _permissionType
        );
    }

    public static implicit operator AccessControlListEntryDescriptor(AccessControlListEntryDescriptorBuilder builder)
        => builder.Build();
}