using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.ConfluentCloud;
using KafkaJanitor.Tests.Builders;
using KafkaJanitor.Tests.TestDoubles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace KafkaJanitor.Tests.Infrastructure.ConfluentCloud;

public class TestConfluentGateway
{
    #region create service account

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("       ")]
    public async Task create_service_account_throws_expected_exception_when_name_is_invalid(string? invalid)
    {
        var sut = A.ConfluentGateway.Build();
        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateServiceAccount(invalid!, "dummy", CancellationToken.None));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("       ")]
    public async Task create_service_account_throws_expected_exception_when_description_is_invalid(string? invalid)
    {
        var sut = A.ConfluentGateway.Build();
        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateServiceAccount("dummy", invalid!, CancellationToken.None));
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task create_service_account_returns_expected_service_account_id(string expected)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "iam/v2/service-accounts",
                handler: () => Results.Content(
                    content: $@"{{ ""id"": ""{expected}"" }}",
                    contentType: "application/json",
                    contentEncoding: Encoding.UTF8
                )))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        var result = await sut.CreateServiceAccount(
            name: expected,
            description: "dummy",
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(ServiceAccountId.Parse(expected), result);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task create_service_account_sends_expected_display_name(string expected)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var sentDisplayName = "...";

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "iam/v2/service-accounts",
                handler: ([FromBody] Dictionary<string, string> payload) =>
                {
                    sentDisplayName = payload["display_name"];

                    return Results.Content(
                        content: $@"{{ ""id"": ""dummy"" }}",
                        contentType: "application/json",
                        contentEncoding: Encoding.UTF8
                    );
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateServiceAccount(
            name: expected,
            description: "dummy",
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(expected, sentDisplayName);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task create_service_account_sends_expected_description(string expected)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var sentDescription = "...";

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "iam/v2/service-accounts",
                handler: ([FromBody] Dictionary<string, string> payload) =>
                {
                    sentDescription = payload["description"];

                    return Results.Content(
                        content: $@"{{ ""id"": ""dummy"" }}",
                        contentType: "application/json",
                        contentEncoding: Encoding.UTF8
                    );
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateServiceAccount(
            name: "dummy",
            description: expected,
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(expected, sentDescription);
    }

    [Fact]
    public async Task create_service_account_throws_expected_exception_when_id_is_missing_from_response()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "iam/v2/service-accounts",
                handler: () => Results.Content(
                    content: $@"{{ ""dummy"": ""dummy"" }}",
                    contentType: "application/json",
                    contentEncoding: Encoding.UTF8
                )))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await Assert.ThrowsAsync<ConfluentGatewayException>(() => sut.CreateServiceAccount(
            name: "dummy",
            description: "dummy",
            cancellationToken: cancellationTokenSource.Token
        ));
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task create_service_account_uses_expected_authorization_header(string headerValue)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var sentHeader = "...";

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "iam/v2/service-accounts",
                handler: ([FromHeader] string? authorization) =>
                {
                    sentHeader = authorization;
                    return Results.Content(
                        content: $@"{{ ""id"": ""dummy"" }}",
                        contentType: "application/json",
                        contentEncoding: Encoding.UTF8
                    );
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .WithConfluentCredentialsProvider(new StubConfluentCredentialsProvider(accountApiKey: headerValue))
            .Build();

        await sut.CreateServiceAccount(
            name: "dummy",
            description: "dummy",
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal($"Basic {headerValue}", sentHeader);
    }

    [Fact]
    public async Task create_service_account_uses_expected_host_url()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        var usedHost = "";

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "iam/v2/service-accounts",
                handler: (HttpRequest request) =>
                {
                    usedHost = $"{request.Scheme}://{request.Host}";
                    return Results.Content(
                        content: $@"{{ ""id"": ""dummy"" }}",
                        contentType: "application/json",
                        contentEncoding: Encoding.UTF8
                    );
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateServiceAccount(
            name: "dummy",
            description: "dummy",
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal("https://api.confluent.cloud", usedHost);
    }

    #endregion

    #region create topic

    [Theory]
    [InlineData("foo", "bar")]
    [InlineData("bar", "baz")]
    [InlineData("baz" ,"qux")]
    public async Task create_topic_sends_expected_topic_name_when_private(string capabilityRootId, string name)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var sentElement = (JsonElement?)null;

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromBody] JsonDocument payload) =>
                {
                    sentElement = payload.RootElement.GetProperty("topic_name");
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateTopic(
            clusterId: A.Cluster.Build().Id,
            topic: A.TopicName
                .WithCapabilityRootId(capabilityRootId)
                .WithName(name)
                .WithIsPublic(false),
            partition: TopicPartition.From(1),
            retention: TopicRetention.FromMilliseconds(1),
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(JsonValueKind.String, sentElement?.ValueKind);
        Assert.Equal($"{capabilityRootId}.{name}", sentElement?.GetString());
    }

    [Theory]
    [InlineData("foo", "bar")]
    [InlineData("bar", "baz")]
    [InlineData("baz" ,"qux")]
    public async Task create_topic_sends_expected_topic_name_when_public(string capabilityRootId, string name)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var sentElement = (JsonElement?)null;

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromBody] JsonDocument payload) =>
                {
                    sentElement = payload.RootElement.GetProperty("topic_name");
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateTopic(
            clusterId: A.Cluster.Build().Id,
            topic: A.TopicName
                .WithCapabilityRootId(capabilityRootId)
                .WithName(name)
                .WithIsPublic(true),
            partition: TopicPartition.From(1),
            retention: TopicRetention.FromMilliseconds(1),
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(JsonValueKind.String, sentElement?.ValueKind);
        Assert.Equal($"pub.{capabilityRootId}.{name}", sentElement?.GetString());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(100)]
    public async Task create_topic_sends_expected_partition_count(uint expected)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        var sentElement = (JsonElement?) null;

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromBody] JsonDocument payload) =>
                {
                    sentElement = payload.RootElement.GetProperty("partition_count");
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateTopic(
            clusterId: A.ClusterId,
            topic: A.TopicName,
            partition: TopicPartition.From(expected),
            retention: TopicRetention.FromMilliseconds(1),
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(JsonValueKind.Number, sentElement?.ValueKind);
        Assert.Equal(expected, sentElement?.GetUInt32());
    }

    [Fact]
    public async Task create_topic_sends_expected_default_replication_factor()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        var sentElement = (JsonElement?) null;

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromBody] JsonDocument payload) =>
                {
                    sentElement = payload.RootElement.GetProperty("replication_factor");
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateTopic(
            clusterId: A.ClusterId,
            topic: A.TopicName,
            partition: TopicPartition.From(1),
            retention: TopicRetention.FromMilliseconds(1),
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(JsonValueKind.Number, sentElement?.ValueKind);
        Assert.Equal(3, sentElement?.GetInt32());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(100)]
    public async Task create_topic_sends_expected_retention(uint expectedRetention)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        var configsElement = (JsonElement?) null;

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromBody] JsonDocument payload) =>
                {
                    configsElement = payload.RootElement.GetProperty("configs");
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateTopic(
            clusterId: A.ClusterId,
            topic: A.TopicName,
            partition: TopicPartition.From(1),
            retention: TopicRetention.FromMilliseconds(expectedRetention),
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.NotNull(configsElement);
        Assert.Equal(JsonValueKind.Array, configsElement?.ValueKind);

        var configs = configsElement?.EnumerateArray() ?? new JsonElement.ArrayEnumerator();
        Assert.True(configs.All(x => x.ValueKind == JsonValueKind.Object));

        var configObjects  = configs.Select(x => new
        {
            Name = x.GetProperty("name").GetString(), 
            Value = x.GetProperty("value").GetString()
        });

        var retentionConfig = Assert.Single(configObjects, x => x.Name == "retention.ms");
        Assert.Equal(expectedRetention.ToString(), retentionConfig.Value);
    }

    [Fact]
    public async Task create_topic_sends_expected_retention_when_infinite()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        var configsElement = (JsonElement?) null;

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromBody] JsonDocument payload) =>
                {
                    configsElement = payload.RootElement.GetProperty("configs");
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateTopic(
            clusterId: A.ClusterId,
            topic: A.TopicName,
            partition: TopicPartition.From(1),
            retention: TopicRetention.Infinite,
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.NotNull(configsElement);
        Assert.Equal(JsonValueKind.Array, configsElement?.ValueKind);

        var configs = configsElement?.EnumerateArray() ?? new JsonElement.ArrayEnumerator();
        Assert.True(configs.All(x => x.ValueKind == JsonValueKind.Object));

        var configObjects  = configs.Select(x => new
        {
            Name = x.GetProperty("name").GetString(), 
            Value = x.GetProperty("value").GetString()
        });

        var retentionConfig = Assert.Single(configObjects, x => x.Name == "retention.ms");
        Assert.Equal("-1", retentionConfig.Value);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task create_topic_sends_expected_cluster_id(string expectedClusterId)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        var sentClusterId = "...";

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromRoute] string clusterId) =>
                {
                    sentClusterId = clusterId;
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateTopic(
            clusterId: ClusterId.Parse(expectedClusterId),
            topic: A.TopicName,
            partition: TopicPartition.From(1),
            retention: TopicRetention.FromMilliseconds(1),
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal(expectedClusterId, sentClusterId);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task create_topic_uses_expected_api_key(string expectedHeaderValue)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var sentHeader = "...";

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: ([FromHeader] string? authorization) =>
                {
                    sentHeader = authorization;
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .WithConfluentCredentialsProvider(new StubConfluentCredentialsProvider(accountApiKey: "not-this-one", clusterApiKey: expectedHeaderValue))
            .Build();

        await sut.CreateTopic(
            clusterId: A.ClusterId,
            topic: A.TopicName,
            partition: TopicPartition.From(1),
            retention: TopicRetention.FromMilliseconds(1),
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal($"Basic {expectedHeaderValue}", sentHeader);
    }

    [Fact]
    public async Task create_topic_throws_expected_exception_when_no_api_key_was_found_for_cluster()
    {
        var sut = A.ConfluentGateway
            .WithConfluentCredentialsProvider(StubConfluentCredentialsProvider.AsEmpty())
            .Build();

        await Assert.ThrowsAsync<ConfluentGatewayException>(() => sut.CreateTopic(
            clusterId: A.ClusterId,
            topic: A.TopicName,
            partition: A.TopicPartition,
            retention: A.TopicRetention,
            cancellationToken: CancellationToken.None
        ));
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task create_topic_uses_expected_host_url(string expectedHost)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        var usedHost = "";

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/topics",
                handler: (HttpRequest request) =>
                {
                    usedHost = $"{request.Scheme}://{request.Host}";
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .WithClusterRepository(A.ClusterRepositoryStub
                .WithCluster(_ => _
                    .WithAdminApiEndpoint($"https://{expectedHost}")
                )
                .Build()
            )
            .Build();

        await sut.CreateTopic(
            clusterId: A.ClusterId,
            topic: A.TopicName,
            partition: A.TopicPartition,
            retention: A.TopicRetention,
            cancellationToken: cancellationTokenSource.Token
        );

        Assert.Equal($"https://{expectedHost}", usedHost);
    }

    #endregion

    #region create acl entry

    [Fact]
    public async Task create_acl_entry_sends_expected_resource_type()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var sentValue = (string?)null;

        using var externalApi = new FakeExternalApiBuilder()
            .WithEndpoint(x => x.MapPost(
                pattern: "kafka/v3/clusters/{clusterId}/acls",
                handler: ([FromBody] Dictionary<string, string> payload) =>
                {
                    sentValue = payload["resource_type"];
                    return Results.NoContent();
                }
            ))
            .Build();

        var sut = A.ConfluentGateway
            .WithHttpClient(externalApi.CreateClient())
            .Build();

        await sut.CreateACLEntry(
            A.ServiceAccountId,
            A.AccessControlListEntryDescriptor,
            cancellationTokenSource.Token
        );

        Assert.Equal("TOPIC", sentValue);
    }


    // test: create acl entry (resource type)
    // test: create acl entry (resource name)
    // test: create acl entry (pattern type)
    // test: create acl entry (principal/service account)
    // test: create acl entry (host)
    // test: create acl entry (operation)
    // test: create acl entry (permission)
    // test: using expected api key
    // test: uses expected host in url

    #endregion

    #region create cluster api key

    // test: create expected api key (display name)
    // test: create expected api key (description)
    // test: create expected api key (service account)
    // test: create expected api key (cluster)
    // test: using expected api key
    // test: uses expected host in url

    #endregion
}