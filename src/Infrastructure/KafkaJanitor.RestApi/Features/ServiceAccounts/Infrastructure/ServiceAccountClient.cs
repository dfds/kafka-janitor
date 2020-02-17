using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Tika.RestClient;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Features.ServiceAccounts.Infrastructure
{
    public class ServiceAccountClient : IServiceAccountClient
    {
        private readonly IRestClient _tikaClient;

        public ServiceAccountClient(IRestClient tikaClient)
        {
            _tikaClient = tikaClient;
        }
        public async Task<ServiceAccount> CreateServiceAccount(Capability capability)
        {
            return await _tikaClient.ServiceAccounts.CreateAsync(new ServiceAccountCreate {
                name = $"{capability.Name}_sa",
                description = "Creating with TikaService using KafkaJanitor"
            });
        }
    }
}