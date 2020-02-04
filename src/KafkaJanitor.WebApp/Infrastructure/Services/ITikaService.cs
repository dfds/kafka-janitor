using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;
using ServiceAccount = Tika.RestClient.Features.ServiceAccounts.Models.ServiceAccount;

namespace KafkaJanitor.WebApp.Infrastructure.Services
{
    public interface ITikaService
    {
        Task CreateTopic();
        Task<ServiceAccount> CreateServiceAccount(Capability capability);
        Task CreateAcl(Capability capability, ServiceAccount serviceAccount);
    }
}