using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Specifications
{
    public class RestApiHostBuilder
    {
        private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors = new Dictionary<Type, ServiceDescriptor>();

        public RestApiHostBuilder WithService(Type serviceType, object serviceInstance)
        {
            _serviceDescriptors.Remove(serviceType);
            _serviceDescriptors.Add(serviceType, ServiceDescriptor.Singleton(serviceType, serviceInstance));

            return this;
        }
        public RestApiHostBuilder WithService<TService>(TService serviceInstance)
        {
            return WithService(typeof(TService), serviceInstance);
        }
        public async Task<IHost> CreateAsync()
        {
            
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                    webHost.ConfigureTestServices(services =>
                    {
                        _serviceDescriptors
                            .Values
                            .ToList()
                            .ForEach(serviceOverride => services.Replace(serviceOverride));
                    });
                    // Look for controllers in the same assembly as the startup class
                    webHost.UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).Assembly.GetName().Name);
                });

            var host = await hostBuilder.StartAsync();
            
            
            return host;
        }
    }
}