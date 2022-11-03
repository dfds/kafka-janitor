using System;
using System.Collections.Generic;
using System.Linq;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.TestDoubles;

namespace KafkaJanitor.Tests.Builders;

public class StubClusterRepositoryBuilder
{
    private IEnumerable<Cluster> _clusters;

    public StubClusterRepositoryBuilder()
    {
        _clusters = Enumerable.Empty<Cluster>();
    }

    public StubClusterRepositoryBuilder WithClusters(params Cluster[] clusters)
    {
        _clusters = clusters;
        return this;
    }

    public StubClusterRepositoryBuilder WithCluster(Action<ClusterBuilder> builderModifier)
    {
        var builder = new ClusterBuilder();
        builderModifier(builder);

        return WithClusters(builder.Build());
    }

    public StubClusterRepository Build()
    {
        return new StubClusterRepository(_clusters);
    }

    public static implicit operator StubClusterRepository(StubClusterRepositoryBuilder builder)
        => builder.Build();
}