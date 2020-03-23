using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class InMemoryVault : IVault
    {
        private readonly List<ApiCredentials> _apiCredentials;
        private readonly List<ApiCredentials> _kafkaConfigurationCredentials;
        public InMemoryVault()
        {
            _apiCredentials = new List<ApiCredentials>();
            _kafkaConfigurationCredentials = new List<ApiCredentials>();
        }

        public Task EnsureConnection()
        {
            return Task.CompletedTask;
        }

        public Task AddApiCredentials(Capability capability, ApiCredentials apiCredentials)
        {
            _apiCredentials.Add(apiCredentials);
            
            return Task.CompletedTask;
        }

        public Task AddKafkaConfiguration(Capability capability, ApiCredentials apiCredentials)
        {
            _kafkaConfigurationCredentials.Add(apiCredentials);
            
            return Task.CompletedTask;
        }
    }
}