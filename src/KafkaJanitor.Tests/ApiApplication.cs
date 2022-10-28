using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace KafkaJanitor.Tests;

public class ApiApplication : WebApplicationFactory<Program>
{
    private readonly List<Action<IServiceCollection>> _serviceCollectionModifiers = new List<Action<IServiceCollection>>();
    private readonly Dictionary<string, string> _customConfiguration = new Dictionary<string, string>();
    private readonly string _environment;

    public ApiApplication(bool configureForProduction = true)
    {
        _environment = configureForProduction
            ? Environments.Production
            : Environments.Development;
    }

    public ApiApplication ConfigureService(Action<IServiceCollection> cfg)
    {
        _serviceCollectionModifiers.Add(cfg);
        return this;
    }

    public ApiApplication RemoveService<TServiceType>()
    {
        ConfigureService(services =>
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(TServiceType));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        });

        return this;
    }

    public ApiApplication ReplaceService<TServiceType>(TServiceType? implementation)
    {
        ConfigureService(services =>
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(TServiceType));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var newDescriptor = ServiceDescriptor.Describe(
                serviceType: typeof(TServiceType),
                implementationFactory: _ => implementation!,
                lifetime: ServiceLifetime.Singleton
                //lifetime: descriptor?.Lifetime ?? ServiceLifetime.Transient
            );

            services.Add(newDescriptor);
        });

        return this;
    }

    public ApiApplication ReplaceConfiguration(string key, string value)
    {
        _customConfiguration[key] = value;
        return this;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_environment);
        builder.ConfigureLogging(cfg =>
        {
            cfg.ClearProviders();
            cfg.Services.RemoveAll<ILogger>();
            cfg.Services.RemoveAll<ILoggerFactory>();
            cfg.Services.AddTransient<ILoggerFactory, NullLoggerFactory>();
        });

        builder.ConfigureAppConfiguration(x =>
        {
            x.Sources.Clear();

            if (_customConfiguration.Any())
            {
                x.AddInMemoryCollection(_customConfiguration);
            }
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHealthCheck>();
            services.RemoveAll<IHostedService>();
        });

        builder.ConfigureTestServices(collection =>
        {
            _serviceCollectionModifiers.ForEach(cfg => cfg(collection));
        });
    }
}