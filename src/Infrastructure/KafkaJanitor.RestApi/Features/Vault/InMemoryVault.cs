using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class InMemoryVault : IVault
    {
        private readonly Dictionary<string, List<ApiCredentials>> _data;

        public InMemoryVault()
        {
            _data = new Dictionary<string, List<ApiCredentials>>();
        }

        public Task EnsureConnection()
        {
            return Task.CompletedTask;
        }

        public Task AddApiCredentials(Capability capability, ApiCredentials apiCredentials, string clusterId)
        {
            if (_data.ContainsKey(clusterId))
            {
                _data[clusterId].Add(apiCredentials);
            }
            else
            {
                _data.Add(clusterId, new List<ApiCredentials>());
                _data[clusterId].Add(apiCredentials);
            }
            
            return Task.CompletedTask;
        }
    }
}