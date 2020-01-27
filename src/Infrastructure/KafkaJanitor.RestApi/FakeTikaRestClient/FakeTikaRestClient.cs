using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.FakeTikaRestClient.Models;

namespace KafkaJanitor.RestApi.FakeTikaRestClient
{
    public class FakeTikaRestClient : ITikaRestClient
    {
        List<string> topics = new List<string>();
        public Task<bool> Exists(string topicName)
        {
            return Task.FromResult(topics.Any(t => t == topicName));
        }

        public Task<Topic> Add(string topicName)
        {
            topics.Add(topicName);

            return Task.FromResult(new Topic(topicName, 3));
        }
    }
}