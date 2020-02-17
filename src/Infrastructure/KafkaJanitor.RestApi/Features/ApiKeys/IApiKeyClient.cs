using System.Threading.Tasks;
using Tika.RestClient.Features.ApiKeys.Models;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Features.ApiKeys
{
    public interface IApiKeyClient
    {
        Task<ApiKey> CreateApiKeyPair(ServiceAccount serviceAccount);
    }
}