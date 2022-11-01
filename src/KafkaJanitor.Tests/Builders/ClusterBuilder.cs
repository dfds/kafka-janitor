﻿using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class ClusterBuilder
{
    private ClusterId _id;
    private string _name;
    private string _bootstrapEndpoint;

    public ClusterBuilder()
    {
        _id = ClusterId.Parse("foo");
        _name = "bar";
        _bootstrapEndpoint = "baz";
    }

    public ClusterBuilder WithId(ClusterId id)
    {
        _id = id;
        return this;
    }

    public ClusterBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ClusterBuilder WithBootstrapEndpoint(string bootstrapEndpoint)
    {
        _bootstrapEndpoint = bootstrapEndpoint;
        return this;
    }

    public Cluster Build()
    {
        return new Cluster(_id, _name, _bootstrapEndpoint);
    }

    public static implicit operator Cluster(ClusterBuilder builder)
        => builder.Build();
}