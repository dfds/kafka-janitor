using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaJanitor.Tests;

public class FakeExternalApiBuilder
{
    private readonly List<Action<IEndpointRouteBuilder>> _routeDefinitions = new List<Action<IEndpointRouteBuilder>>();

    public FakeExternalApiBuilder WithEndpoint(Action<IEndpointRouteBuilder> route)
    {
        _routeDefinitions.Add(route);
        return this;
    }

    public FakeExternalApi Build()
    {
        return new FakeExternalApi(_routeDefinitions);
    }

    public class FakeExternalApi : IDisposable
    {
        private readonly IEnumerable<Action<IEndpointRouteBuilder>> _routeDefinitions;
        private TestServer? _testServer;
        private HttpClient? _httpClient;

        public FakeExternalApi(IEnumerable<Action<IEndpointRouteBuilder>> routeDefinitions)
        {
            _routeDefinitions = routeDefinitions;
        }

        private WebHostBuilder CreateWebHostBuilder()
        {
            var builder = new WebHostBuilder();
            builder.ConfigureServices(services =>
            {
                services.AddRouting();
            });

            builder.Configure(app =>
            {
                app.UseRouting();

                foreach (var routeDefinition in _routeDefinitions)
                {
                    app.UseEndpoints(routeDefinition);
                }
            });

            return builder;
        }

        public HttpClient CreateClient(string? baseUrl = null)
        {
            var webHostBuilder = CreateWebHostBuilder();
            _testServer = new TestServer(webHostBuilder);
            _httpClient = _testServer.CreateClient();

            if (baseUrl is not null)
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }

            return _httpClient;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _testServer?.Dispose();
        }
    }
}