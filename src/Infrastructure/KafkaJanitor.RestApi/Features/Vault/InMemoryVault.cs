using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class InMemoryVault : IVault
    {
        public readonly Dictionary<string, List<ApiCredentials>> RootIdToApiCredentials;
        public InMemoryVault()
        {
            RootIdToApiCredentials = new Dictionary<string, List<ApiCredentials>>();
        }

        public Task EnsureConnection()
        {
            return Task.CompletedTask;
        }

        public Task AddApiCredentials(
            Capability capability, 
            ApiCredentials apiCredentials
        )
        {
            if (RootIdToApiCredentials.ContainsKey(capability.RootId) == false)
            {
                RootIdToApiCredentials.Add(capability.RootId,new List<ApiCredentials>());
            }
            
            RootIdToApiCredentials[capability.RootId].Add(apiCredentials);
            
            return Task.CompletedTask;
        }
    }
}