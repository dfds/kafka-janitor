using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.FakeTikaRestClient.Models;

namespace KafkaJanitor.RestApi.FakeTikaRestClient
{
    public class FakeTikaRestClient : ITikaRestClient
    {
        List<string> topicStrings = new List<string>();
        public Task<bool> ExistsAsync(string topicName)
        {
            return Task.FromResult(topicStrings.Any(t => t == topicName));
        }

        public Task<Topic> AddAsync(string topicName)
        {
            topicStrings.Add(topicName);

            return Task.FromResult(new Topic(topicName, 3));
        }

        public Task<IEnumerable<Topic>> GetAllAsync()
        {
            var topics = topicStrings
                .Select(topicName => new Topic(topicName, 3))
                .ToList();
            
            return Task.FromResult<IEnumerable<Topic>>(topics);
        }
    }
}