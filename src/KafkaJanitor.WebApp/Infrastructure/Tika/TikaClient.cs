using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Infrastructure.Tika.Model;
using Newtonsoft.Json;

namespace KafkaJanitor.WebApp.Infrastructure.Tika
{
    public class TikaClient : ITikaClient
    {
        private HttpClient _httpClient;
        private TikaOptions _options;

        public TikaClient(HttpClient httpClient = null, TikaOptions options = null)
        {
            _httpClient = httpClient ?? new HttpClient() {BaseAddress = new System.Uri(options?.TIKA_API_ENDPOINT, System.UriKind.Absolute)};
            _options = options;
        }
        public async Task<IEnumerable<ApiKey>> GetApiKeys()
        {
            var result = await _httpClient.GetAsync($"{_options.TIKA_API_ENDPOINT}/api-keys");

            return await Parse<IEnumerable<ApiKey>>(result);
        }

        public async Task<ApiKey> CreateApiKey(string serviceAccountId, string description = null)
        {
            var result = await _httpClient.PostAsync($"{_options.TIKA_API_ENDPOINT}/api-keys", await PayloadToJson(new
            {
                ServiceAccountId = serviceAccountId,
                Description = description
            }));

            return await Parse<ApiKey>(result);
        }

        public async Task DeleteApiKey(string key)
        {
            var result = await _httpClient.GetAsync($"{_options.TIKA_API_ENDPOINT}/api-keys");
            result.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<ServiceAccount>> GetServiceAccounts()
        {
            var result = await _httpClient.GetAsync($"{_options.TIKA_API_ENDPOINT}/service-accounts");

            return await Parse<IEnumerable<ServiceAccount>>(result);
        }

        public async Task<ServiceAccount> CreateServiceAccount(string name, string description = null)
        {
            var result = await _httpClient.PostAsync($"{_options.TIKA_API_ENDPOINT}/service-accounts", await PayloadToJson(new
            {
                Name = name,
                Description = description
            }));

            return await Parse<ServiceAccount>(result);
        }

        public async Task DeleteServiceAccount(string id)
        {
            var result = await _httpClient.DeleteAsync($"{_options.TIKA_API_ENDPOINT}/service-accounts/{id}");
            result.EnsureSuccessStatusCode();
        }

        public async Task GetTopics()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateTopic()
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteTopic()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Acl>> GetAcls()
        {
            var result = await _httpClient.GetAsync($"{_options.TIKA_API_ENDPOINT}/access-control-lists");

            return await Parse<IEnumerable<Acl>>(result);
        }

        public async Task CreateAcl(string serviceAccountId, bool allow, string operation, string topicPrefix, string consumerGroupPrefix)
        {
            var result = await _httpClient.PostAsync($"{_options.TIKA_API_ENDPOINT}/access-control-lists", await PayloadToJson(new
            {
                ServiceAccountId = serviceAccountId,
                Allow = allow,
                Operation = operation,
                TopicPrefix = topicPrefix,
                ConsumerGroupPrefix = consumerGroupPrefix
            }));

            result.EnsureSuccessStatusCode();
        }

        public async Task DeleteAcl(string serviceAccountId, bool allow, string operation, string topicPrefix, string consumerGroupPrefix)
        {
            var result = await _httpClient.PostAsync($"{_options.TIKA_API_ENDPOINT}/access-control-lists/delete", await PayloadToJson(new
            {
                ServiceAccountId = serviceAccountId,
                Allow = allow,
                Operation = operation,
                TopicPrefix = topicPrefix,
                ConsumerGroupPrefix = consumerGroupPrefix
            }));

            result.EnsureSuccessStatusCode();
        }

        private async Task<HttpContent> PayloadToJson(object input)
        {
            return new StringContent(
                content: JsonConvert.SerializeObject(input),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
        
        private async Task<T> Parse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<T>(content);
            return data;
        }
    }
}