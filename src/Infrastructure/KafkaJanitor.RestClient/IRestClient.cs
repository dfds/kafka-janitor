using KafkaJanitor.RestClient.Features.Topics;

namespace KafkaJanitor.RestClient
{
    public interface IRestClient
    {
        ITopicsClient Topics { get; }
    }
}