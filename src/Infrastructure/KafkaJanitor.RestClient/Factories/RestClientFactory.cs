using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace KafkaJanitor.RestClient.Factories
{
    public static class RestClientFactory
    {
        public static IRestClient Create(HttpClient httpClient)
        {
            return new Client(httpClient);
        }
        
        public static IRestClient CreateFromConfiguration(HttpClient httpClient, IOptions<ClientOptions> options)
        {
            if (options.Value?.KAFKAJANITOR_API_ENDPOINT == null)
            {
                throw new KafkaJanitorRestClientInvalidConfigurationException("KAFKAJANITOR_API_ENDPOINT");
            }
            httpClient.BaseAddress = new Uri(options.Value?.KAFKAJANITOR_API_ENDPOINT);
            return new Client(httpClient);
        }
    }
    
    public class KafkaJanitorRestClientInvalidConfigurationException : Exception
    {
        public KafkaJanitorRestClientInvalidConfigurationException() : base("RestClientFactory was unable to find the necessary configuration to create a RestClient. Please refer to the configuration section of Tika.RestClient's README.")
        {
            
        }

        public KafkaJanitorRestClientInvalidConfigurationException(string message) : base($"RestClientFactory was unable to find the necessary configuration for '{message}' to create a RestClient. Please refer to the configuration section of Tika.RestClient's README.")
        {
            
        }
        
        public KafkaJanitorRestClientInvalidConfigurationException(string message, Exception inner) : base($"RestClientFactory was unable to find the necessary configuration for '{message}' to create a RestClient. Please refer to the configuration section of Tika.RestClient's README.", inner)
        {
            
        }
    }
}