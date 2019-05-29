using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaJanitor.WebApp.Models;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class TopicRepository : ITopicRepository, IDisposable
    {
        private const short DefaultReplicationFactor = 3;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);
        private readonly IAdminClient _client;

        public TopicRepository(KafkaConfiguration config)
        {
            _client = new AdminClientBuilder(config.GetConfiguration()).Build();
        }

        private Metadata GetMetaData() => _client.GetMetadata(_timeout);
        
        private int GetBrokerCount() => GetMetaData().Brokers.Count;

        public Task<IEnumerable<Topic>> GetAll()
        {
            var metaData = GetMetaData();
            var topics = metaData.Topics.Select(x => new Topic(x.Topic, x.Partitions.Count));
            
            return Task.FromResult(topics);
        }

        public async Task<bool> Exists(string topicName)
        {
            var topics = await GetAll();
            return topics.Any(x => x.Name == topicName);
        }
        
        public async Task Add(Topic topic)
        {
            var brokers = GetBrokerCount();
            
            var replicationFactor = (short) (brokers < DefaultReplicationFactor
                ? brokers
                : DefaultReplicationFactor);

            var topicSpec = new TopicSpecification
            {
                Name = topic.Name,
                NumPartitions = topic.Partitions,
                ReplicationFactor = replicationFactor
            };

            await _client.CreateTopicsAsync(new[] {topicSpec});
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}