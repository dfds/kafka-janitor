using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.Builders;
using Xunit;

namespace KafkaJanitor.Tests.Infrastructure.ConfluentCloud;

public class TestJsonBasedConfluentCredentialsProvider
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("      ")]
    public async Task returns_expected_account_admin_key_when_empty(string? content)
    {
        var sut = A.JsonBasedConfluentCredentialsProvider
            .WithContent(content!)
            .Build();

        var result = await sut.GetAccountAdminApiKey();

        Assert.Empty(result);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public async Task returns_expected_account_admin_key(string expected)
    {
        var sut = A.JsonBasedConfluentCredentialsProvider
            .WithContent($@"{{ ""account"": ""{expected}"" }}")
            .Build();

        var result = await sut.GetAccountAdminApiKey();

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task returns_expected_cluster_api_key_when_not_cluster_defined()
    {
        var sut = A.JsonBasedConfluentCredentialsProvider
            .WithContent($@"{{ ""account"": ""foo"" }}")
            .Build();

        var result = await sut.FindAdminApiKeyForCluster(ClusterId.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task returns_expected_cluster_api_key_when_defined_clusters_are_empty()
    {
        var sut = A.JsonBasedConfluentCredentialsProvider
            .WithContent($@"{{ ""account"": ""foo"", ""clusters"": [] }}")
            .Build();

        var result = await sut.FindAdminApiKeyForCluster(ClusterId.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task returns_expected_cluster_api_key_when_defined_clusters_does_not_contain_cluster_id()
    {
        var sut = A.JsonBasedConfluentCredentialsProvider
            .WithContent($@"{{ ""account"": ""dummy"", ""clusters"": [{{ ""id"": ""foo"" }}] }}")
            .Build();

        var result = await sut.FindAdminApiKeyForCluster(ClusterId.Parse("bar"));

        Assert.Null(result);
    }

    [Fact]
    public async Task returns_expected_cluster_api_key()
    {
        var sut = A.JsonBasedConfluentCredentialsProvider
            .WithContent($@"{{ ""account"": ""dummy"", ""clusters"": [{{ ""id"": ""foo"", ""apiKey"": ""bar"" }}] }}")
            .Build();

        var result = await sut.FindAdminApiKeyForCluster(ClusterId.Parse("foo"));

        Assert.Equal("bar", result);
    }
}