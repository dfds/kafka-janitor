using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KafkaJanitor.RestClient.Features.Topics.Models;
using Newtonsoft.Json;

namespace KafkaJanitor.RestClient.Features.Topics
{
    internal class TopicsClient : ITopicsClient
    {
        private readonly HttpClient _httpClient;
        private const string TOPICS_ROUTE = "api/topics/";

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

        public async Task<IEnumerable<Topic>> GetAllAsync(string clusterId)
        {
            var uri = new Uri(_httpClient.BaseAddress + TOPICS_ROUTE, UriKind.Absolute);
            var builder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["clusterId"] = clusterId;
            builder.Query = query.ToString();

            var httpResponseMessage = await _httpClient.GetAsync(
                builder.Uri
            );

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var topics = JsonConvert.DeserializeObject<IEnumerable<Topic>>(content);

            return topics;
        }
        
        public async Task<Topic> DescribeAsync(string topicName, string clusterId)
        {
            var uri = new Uri(TOPICS_ROUTE + topicName, UriKind.Relative);
            var builder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["clusterId"] = clusterId;
            builder.Query = query.ToString();

            var httpResponseMessage = await _httpClient.GetAsync(
                builder.Uri
            );
            httpResponseMessage.EnsureSuccessStatusCode();

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var topicDescription = JsonConvert.DeserializeObject<Topic>(content);

            return topicDescription;
        }
    }
}