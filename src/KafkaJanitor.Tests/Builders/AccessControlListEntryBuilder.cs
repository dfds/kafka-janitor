using System;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class AccessControlListEntryBuilder
{
    private AccessControlListEntryId _id;
    private AccessControlListEntryDescriptor _descriptor;
    private bool _isApplied;

    public AccessControlListEntryBuilder()
    {
        _id = AccessControlListEntryId.New();
        _descriptor = A.AccessControlListEntryDescriptor;
        _isApplied = false;
    }

    public AccessControlListEntryBuilder WithId(AccessControlListEntryId id)
    {
        _id = id;
        return this;
    }

    public AccessControlListEntryBuilder WithDescriptor(AccessControlListEntryDescriptor descriptor)
    {
        _descriptor = descriptor;
        return this;
    }

    public AccessControlListEntryBuilder WithDescriptor(Action<AccessControlListEntryDescriptorBuilder> builderModifier)
    {
        var builder = new AccessControlListEntryDescriptorBuilder();
        builderModifier(builder);

        return WithDescriptor(builder.Build());
    }

    public AccessControlListEntryBuilder WithIsApplied(bool isApplied)
    {
        _isApplied = isApplied;
        return this;
    }

    public AccessControlListEntry Build()
    {
        return new AccessControlListEntry(_id, _descriptor, _isApplied);
    }

    public static implicit operator AccessControlListEntry(AccessControlListEntryBuilder builder)
        => builder.Build();
}