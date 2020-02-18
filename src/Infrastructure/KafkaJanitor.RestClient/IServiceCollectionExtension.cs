using System.Net.Http;
using KafkaJanitor.RestClient.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KafkaJanitor.RestClient
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddKafkaJanitorRestClient(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<ClientOptions>();
            services.AddTransient<IRestClient, Client>(o =>
            {
                return RestClientFactory.CreateFromConfiguration(new HttpClient(),
                    Options.Create(new ClientOptions(configuration))) as Client;
            });
            return services;
        }
    }
}