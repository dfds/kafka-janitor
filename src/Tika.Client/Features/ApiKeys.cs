using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tika.Client.Enablers;
using Tika.Client.Models;

namespace Tika.Client
{
    public class ApiKeys
    {
        private Uri _uri = new Uri("/api-keys", UriKind.Relative);
        private readonly HttpClient _httpClient;

        public ApiKeys(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiKeySet> Create(
            int serviceAccountId,
            string description
        )
        {
            var apiKeySet = await HttpRequest.Post<ApiKeySet>(
                _httpClient,
                _uri,
                new {serviceAccountId = serviceAccountId, description = description}
            );

            return apiKeySet;
        }

        public async Task<IEnumerable<ApiKey>> GetAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = _uri
            };

            var response = await _httpClient.SendAsync(request);

            var deserializedObject = await HttpResponse.ToTypeAsync<IEnumerable<ApiKey>>(response);

            return deserializedObject;
        }

        public async Task DeleteAsync(string id)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_uri + "/" + id, UriKind.Relative)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}