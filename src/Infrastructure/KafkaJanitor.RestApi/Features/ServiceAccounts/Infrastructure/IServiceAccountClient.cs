using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Features.ServiceAccounts.Infrastructure
{
    public interface IServiceAccountClient
    {
        Task<ServiceAccount> CreateServiceAccount(Capability capability);
    }
}