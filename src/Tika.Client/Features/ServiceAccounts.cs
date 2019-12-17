using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tika.Client.Models;

namespace Tika.Client
{
    public class ServiceAccounts
    {
        private readonly HttpClient _httpClient;

        public ServiceAccounts(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceAccount> CreateAsync(string name, string description)
        {
            var serializedContent = JsonConvert.SerializeObject(new {name = name, description = description});
            var stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/service-accounts", UriKind.Relative),
                Content = stringContent
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();


            var serviceAccount = JsonConvert.DeserializeObject<ServiceAccount>(content);

            return serviceAccount;
        }

        public async Task<IEnumerable<ServiceAccount>> GetAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("/service-accounts", UriKind.Relative)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();


            var serviceAccounts = JsonConvert.DeserializeObject<IEnumerable<ServiceAccount>>(content);

            return serviceAccounts;
        }

        public async Task DeleteAsync(int id)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/service-accounts/" + id, UriKind.Relative)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}