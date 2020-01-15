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
        public async Task GetApiKeys()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateApiKey(object payload)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteApiKey(object payload)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<ServiceAccount>> GetServiceAccounts()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
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

        public async Task GetAcls()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateAcl()
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAcl()
        {
            throw new System.NotImplementedException();
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