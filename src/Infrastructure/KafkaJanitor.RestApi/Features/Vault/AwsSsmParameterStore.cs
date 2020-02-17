using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class AwsSsmParameterStore : IVault
    {
        public Task AddApiCredentials(ApiCredentials apiCredentials)
        {
            throw new System.NotImplementedException();
        }
    }
}