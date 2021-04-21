using System.Collections.Generic;
using System.Linq;
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

        public async Task<ApiKey> CreateApiKeyPair(ServiceAccount serviceAccount, string clusterId = null)
        {
            return await _tikaClient.ApiKeys.CreateAsync(new ApiKeyCreate
            {
                Description = "Automatically created during SA flow",
                ServiceAccountId = serviceAccount.Id
            }, clusterId);
        }

        public async Task<IEnumerable<ApiKey>> GetApiKeyPairsForServiceAccount(string serviceAccountId, string clusterId = null)
        {
            var result = await _tikaClient.ApiKeys.GetAllAsync(clusterId);

            return result.Where(ak => ak.Owner == serviceAccountId);
        }
    }
}