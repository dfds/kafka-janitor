using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KafkaJanitor.RestClient.Features.Topics.Models;
using Newtonsoft.Json;

namespace KafkaJanitor.RestClient.Features.Topics
{
    internal class TopicsClient : ITopicsClient
    {
        private readonly HttpClient _httpClient;
        private const string TOPICS_ROUTE = "topics/";

        public TopicsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateAsync(Topic input)
        {
            var payload = JsonConvert.SerializeObject(input);

            var content = new StringContent(
                payload,
                Encoding.UTF8,
                "application/json"
            );

            await _httpClient.PostAsync(
                new Uri(TOPICS_ROUTE, UriKind.Relative),
                content
            );
        }

        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            var httpResponseMessage = await _httpClient.GetAsync(
                new Uri(TOPICS_ROUTE, UriKind.Relative)
            );
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var topics = JsonConvert.DeserializeObject<IEnumerable<Topic>>(content);

            return topics;
        }
    }
}