using System.Collections.Generic;
using System.Threading.Tasks;
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
        
        public async Task AddApiCredentials(ApiCredentials apiCredentials)
        {
            _data.Add(apiCredentials);
        }
    }
}