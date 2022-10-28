using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.Builders;
using KafkaJanitor.Tests.TestDoubles;
using Xunit;

namespace KafkaJanitor.Tests.Routes;

public class TestTopicRoutes
{
    [Fact]
    public async Task get_all_returns_expected_status_code_when_empty() 
    {
        await using var application = new ApiApplication();
        application.ReplaceService<IConfluentGateway>(StubConfluentGateway.AsEmpty());

        using var client = application.CreateClient();

        var response = await client.GetAsync($"api/topics?clusterId={ClusterId.None}");

        Assert.Equal((HttpStatusCode)200, response.StatusCode);
    }

    [Fact]
    public async Task get_all_returns_expected_payload_when_empty()
    {
        await using var application = new ApiApplication();
        application.ReplaceService<IConfluentGateway>(StubConfluentGateway.AsEmpty());

        using var client = application.CreateClient();

        var response = await client.GetAsync($"api/topics?clusterId={ClusterId.None}");

        AssertJson.Equal(
            expected: "[]",
            actual: await response.Content.ReadAsStringAsync()
        );
    }

    [Fact]
    public async Task get_all_returns_expected_status_code_when_NOT_empty()
    {
        await using var application = new ApiApplication();
        
        application.ReplaceService<IConfluentGateway>(StubConfluentGateway.Containing(
            new TopicBuilder()
                .WithName("foo")
                .Build(),
            new TopicBuilder()
                .WithName("bar")
                .Build()
        ));

        using var client = application.CreateClient();

        var response = await client.GetAsync($"api/topics?clusterId={ClusterId.None}");

        AssertJson.Equal(
            expected: @"[
                { ""name"": ""foo"" },
                { ""name"": ""bar"" }
            ]",
            actual: await response.Content.ReadAsStringAsync()
        );
    }

    [Fact]
    public async Task post_returns_expected_status_code_when_payload_is_valid()
    {
        await using var application = new ApiApplication();
        application.ReplaceService<IConfluentGateway>(new FakeConfluentGateway());

        using var client = application.CreateClient();

        var payload = @"{
            ""name"": ""dummy"",
            ""description"": ""dummy"",
            ""clusterId"": ""dummy"",
            ""partitions"": 1,
            ""configurations"": {}
        }";

        var response = await client.PostAsync("api/topics", new StringContent(payload, Encoding.UTF8, "application/json"));

        Assert.Equal((HttpStatusCode) 200 ,response.StatusCode);
    }

    [Fact]
    public async Task post_returns_expected_status_code_when_topic_already_exists()
    {
        await using var application = new ApiApplication();
        
        var fakeTopicGateway = new FakeConfluentGateway();
        fakeTopicGateway.Topics.Add(new Topic("foo"));

        application.ReplaceService<IConfluentGateway>(fakeTopicGateway);

        using var client = application.CreateClient();

        var payload = @"{
            ""name"": ""foo"",
            ""description"": ""dummy"",
            ""clusterId"": ""dummy"",
            ""partitions"": 1,
            ""configurations"": {}
        }";

        var response = await client.PostAsync("api/topics", new StringContent(payload, Encoding.UTF8, "application/json"));

        Assert.Equal((HttpStatusCode) 409 ,response.StatusCode);
    }
}