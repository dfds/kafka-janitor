using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Services
{
    public interface ITikaService
    {
        Task CreateTopic();
        Task<ServiceAccount> CreateServiceAccount(Capability capability);
        Task CreateAcl(Capability capability, ServiceAccount serviceAccount);
    }
}