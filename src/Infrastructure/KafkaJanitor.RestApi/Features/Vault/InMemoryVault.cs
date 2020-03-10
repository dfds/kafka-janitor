using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class InMemoryVault : IVault
    {
        private readonly List<ApiCredentials> _data;

        public InMemoryVault()
        {
            _data = new List<ApiCredentials>();
        }

        public Task EnsureConnection()
        {
            return Task.CompletedTask;
        }

        public Task AddApiCredentials(Capability capability, ApiCredentials apiCredentials)
        {
            _data.Add(apiCredentials);
            
            return Task.CompletedTask;
        }
    }
}