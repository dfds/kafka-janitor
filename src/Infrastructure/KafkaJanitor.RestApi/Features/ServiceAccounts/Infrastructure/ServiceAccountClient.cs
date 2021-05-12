using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
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
        public async Task<ServiceAccount> CreateServiceAccount(Capability capability, string clusterId = null)
        {
            return await _tikaClient.ServiceAccounts.CreateAsync(new ServiceAccountCreateCommand
            {
                name = $"{capability.Name}",
                description = "Creating with TikaService using KafkaJanitor"
            }, clusterId);
        }
        public async Task<ServiceAccount> GetServiceAccount(Capability capability, string clusterId = null)
        {
            var results = await _tikaClient.ServiceAccounts.GetAllAsync(clusterId);
            return results.Single(sa => sa.Name == $"{capability.Name}_sa");
        }
    }
}