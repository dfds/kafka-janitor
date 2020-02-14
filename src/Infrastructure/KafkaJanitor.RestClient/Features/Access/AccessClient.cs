using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KafkaJanitor.RestClient.Features.Access.Models;
using Newtonsoft.Json;

namespace KafkaJanitor.RestClient.Features.Access
{
    public class AccessClient : IAccessClient
    {
        private readonly HttpClient _httpClient;
        private const string ACCESS_ROUTE = "api/access/";

        public AccessClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task RequestAsync(ServiceAccountRequestInput input)
        {
            var payload = JsonConvert.SerializeObject(input);
            
            var content = new StringContent(
                payload,
                Encoding.UTF8,
                "application/json"
            );

            await _httpClient.PostAsync(
                new Uri(ACCESS_ROUTE, UriKind.Relative),
                content
            );
        }
    }
}