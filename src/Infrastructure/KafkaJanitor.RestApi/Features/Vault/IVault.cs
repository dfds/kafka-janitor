using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public interface IVault
    {
        Task EnsureConnection();
        Task AddApiCredentials(Capability capability, ApiCredentials apiCredentials, string clusterId = null);
    }
}