using System.Threading.Tasks;
using KafkaJanitor.RestApi.FakeTikaRestClient.Models;

namespace KafkaJanitor.RestApi.FakeTikaRestClient
{
    public interface ITikaRestClient
    {
        Task<bool> Exists(string topicName);
        Task<Topic> Add(string topicName);
    }
}