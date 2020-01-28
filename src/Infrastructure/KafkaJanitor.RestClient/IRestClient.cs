using KafkaJanitor.RestClient.Features.Topics;

namespace KafkaJanitor.RestClient.Factories
{
    public interface IRestClient
    {
        ITopicsClient Topics { get; }
    }
}