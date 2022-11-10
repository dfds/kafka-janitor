using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KafkaJanitor.Tests.Routes;

public class TestPingPongRoutes
{
    [Fact]
    public async Task get_ping_returns_expected_response()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        await using var application = new ApiApplication();
        using var client = application.CreateClient();

        var response = await client.GetAsync("/system/ping", cancellationTokenSource.Token);

        Assert.Equal(
            expected: (HttpStatusCode) 200,
            actual: response.StatusCode
        );

        Assert.Equal(
            expected: "pong",
            actual: await response.Content.ReadAsStringAsync(cancellationTokenSource.Token)
        );
    }
}