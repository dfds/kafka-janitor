using System.Net.Http;
using KafkaJanitor.RestClient.Factories;
using KafkaJanitor.RestClient.Features.Topics;

namespace KafkaJanitor.RestClient
{
    internal class Client : IRestClient
    {
        public ITopicsClient Topics { get; }

        public Client(HttpClient httpClient)
        {
            Topics = new TopicsClient(httpClient);
        }
    }
}