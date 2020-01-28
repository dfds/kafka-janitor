using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.FakeTikaRestClient.Models;

namespace KafkaJanitor.RestApi.FakeTikaRestClient
{
    public interface ITikaRestClient
    {
        Task<bool> ExistsAsync(string topicName);
        Task<Topic> AddAsync(string topicName);
        Task<IEnumerable<Topic>> GetAllAsync();
    }
}