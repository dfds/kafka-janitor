using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;
using Tika.Client;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class TopicCcloudRepository : ITopicRepository
    {
        private readonly ITikaClient _tikaClient;

        public TopicCcloudRepository(ITikaClient tikaClient)
        {
            _tikaClient = tikaClient;
        }
        public async Task<IEnumerable<Topic>> GetAll()
        {
            var topics = await _tikaClient.GetTopics();
            return topics.Select(val => new Topic(val));
        }

        public async Task Add(Topic topic)
        {
            await _tikaClient.CreateTopic(topic.Name, topic.Partitions.ToString());
        }

        public async Task<bool> Exists(string topicName)
        {
            var topics = await _tikaClient.GetTopics();
            try
            {
                topics.First(to => to.Equals(topicName));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}