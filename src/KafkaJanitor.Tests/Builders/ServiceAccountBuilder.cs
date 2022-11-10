using System.Collections.Generic;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class ServiceAccountBuilder
{
    private ServiceAccountId _id;
    private CapabilityRootId _capabilityRootId;
    private IEnumerable<ClusterApiKey> _clusterApiKeys;

    public ServiceAccountBuilder()
    {
        _id = ServiceAccountId.Parse("foo");
        _capabilityRootId = CapabilityRootId.Parse("bar");
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

    public ServiceAccount Build()
    {
        return new ServiceAccount(_id, _capabilityRootId, _clusterApiKeys);
    }

    public static implicit operator ServiceAccount(ServiceAccountBuilder builder)
        => builder.Build();
}