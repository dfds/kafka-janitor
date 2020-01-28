using System.Net.Http;

namespace KafkaJanitor.RestClient.Factories
{
    public static class RestClientFactory
    {
        public static IRestClient Create(HttpClient httpClient)
        {
            return new Client(httpClient);
        }
    }
}