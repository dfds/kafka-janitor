using System.Net.Http;
using KafkaJanitor.RestClient.Features.Access;
using KafkaJanitor.RestClient.Features.Topics;

namespace KafkaJanitor.RestClient
{
    internal class Client : IRestClient
    {
        public ITopicsClient Topics { get; }
        public IAccessClient Access { get; }

        public Client(HttpClient httpClient)
        {
            Topics = new TopicsClient(httpClient);
            Access = new AccessClient(httpClient);
        }
    }
}