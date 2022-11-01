using System.Collections.Generic;
using System.Linq;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class ServiceAccountBuilder
{
    private ServiceAccountId _id;
    private CapabilityRootId _capabilityRootId;
    private IEnumerable<AccessControlListEntry> _accessControlList;
    private IEnumerable<ClusterApiKey> _clusterApiKeys;

    public ServiceAccountBuilder()
    {
        _id = ServiceAccountId.Parse("foo");
        _capabilityRootId = CapabilityRootId.Parse("bar");
        _accessControlList = ServiceAccount.CreateDefaultAccessControlList(_capabilityRootId);
        _clusterApiKeys = new[] { A.ClusterApiKey.Build() };
    }

    public ServiceAccountBuilder WithCapabilityRootId(CapabilityRootId capabilityRootId)
    {
        _capabilityRootId = capabilityRootId;
        return this;
    }

    public ServiceAccountBuilder WithClusterApiKeys(IEnumerable<ClusterApiKey> clusterApiKeys)
    {
        _clusterApiKeys = clusterApiKeys;
        return this;
    }

    public ServiceAccountBuilder WithClusterApiKeys(params ClusterApiKey[] clusterApiKeys)
    {
        _clusterApiKeys = clusterApiKeys;
        return this;
    }

    public ServiceAccountBuilder WithAccessControlList(IEnumerable<AccessControlListEntry> accessControlList)
    {
        _accessControlList = accessControlList;
        return this;
    }

    public ServiceAccount Build()
    {
        return new ServiceAccount(_id, _capabilityRootId, _accessControlList.ToArray(), _clusterApiKeys);
    }

    public static implicit operator ServiceAccount(ServiceAccountBuilder builder)
        => builder.Build();
}