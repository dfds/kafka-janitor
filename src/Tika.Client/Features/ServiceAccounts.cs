using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tika.Client.Enablers;
using Tika.Client.Models;

namespace Tika.Client
{
    public class ServiceAccounts
    {
        private Uri _uri = new Uri("/service-accounts", UriKind.Relative);
        private readonly HttpClient _httpClient;

        public ServiceAccounts(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceAccount> CreateAsync(string name, string description)
        {
                var serviceAccount = await HttpRequest.Post<ServiceAccount>(
                    _httpClient,
                    _uri,
                    new {name = name, description = description}
                );

                return serviceAccount;
        }

        public async Task<IEnumerable<ServiceAccount>> GetAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = _uri
            };

            var response = await _httpClient.SendAsync(request);
            
            var deserializedObject = await HttpResponse.ToTypeAsync<IEnumerable<ServiceAccount>>(response);

            return deserializedObject;
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