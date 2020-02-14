using KafkaJanitor.RestClient.Features.Access;
using KafkaJanitor.RestClient.Features.Topics;

namespace KafkaJanitor.RestClient
{
    public interface IRestClient
    {
        ITopicsClient Topics { get; }
        IAccessClient Access { get; }
    }
}