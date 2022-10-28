using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KafkaJanitor.Tests.Routes;

public class TestAccessRoutes
{
    [Fact]
    public async Task post_valid_input_returns_expected_status_code()
    {
        await using var application = new ApiApplication();
        using var client = application.CreateClient();

        var payload = @"{
            ""CapabilityName"": ""dummy"",
            ""CapabilityId"": ""dummy"",
            ""CapabilityRootId"": ""dummy"",
            ""TopicPrefix"" ""dummy"",
            ""ClusterId"": ""dummy""
        }";

        var response = await client.PostAsync(
            requestUri: "api/access/request",
            content: new StringContent(
                content: payload,
                encoding: Encoding.UTF8, mediaType:
                "application/json"
            )
        );

        Assert.Equal((HttpStatusCode) 200, response.StatusCode);
    }
}