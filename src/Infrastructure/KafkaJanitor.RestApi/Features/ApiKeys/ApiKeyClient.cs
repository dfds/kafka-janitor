using System.Threading.Tasks;
using Tika.RestClient;
using Tika.RestClient.Features.ApiKeys.Models;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Features.ApiKeys
{
    public class ApiKeyClient : IApiKeyClient
    {
        private readonly IRestClient _tikaClient;

        public ApiKeyClient(IRestClient tikaClient)
        {
            _tikaClient = tikaClient;
        }

        public async Task<ApiKey> CreateApiKeyPair(ServiceAccount serviceAccount)
        {
            return await _tikaClient.ApiKeys.CreateAsync(new ApiKeyCreate
            {
                Description = "Automatically created during SA flow",
                ServiceAccountId = serviceAccount.Id
            });
        }
    }
}