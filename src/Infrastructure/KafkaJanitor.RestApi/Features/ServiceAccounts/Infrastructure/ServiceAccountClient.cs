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
        public async Task<ServiceAccount> CreateServiceAccount(Capability capability)
        {
            return await _tikaClient.ServiceAccounts.CreateAsync(new ServiceAccountCreate {
                name = $"{capability.Name}_sa",
                description = "Creating with TikaService using KafkaJanitor"
            });
        }
        public async Task<ServiceAccount> GetServiceAccount(Capability capability)
        {
            var results = await _tikaClient.ServiceAccounts.GetAllAsync();
            return results.First(sa => sa.Name == $"{capability.Name}_sa");
        }
    }
}