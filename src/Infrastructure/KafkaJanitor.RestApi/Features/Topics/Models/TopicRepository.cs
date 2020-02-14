using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;

namespace KafkaJanitor.RestApi.Features.Topics.Models
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
            var topics = metaData.Topics.Select(x => new Topic {Name = x.Topic, Partitions = x.Partitions.Count});
            
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
    
    public class KafkaConfiguration
    {
        private const string KEY_PREFIX = "KAFKA_JANITOR_";
        private readonly IConfiguration _configuration;

        public KafkaConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string Key(string keyName) => string.Join("", KEY_PREFIX, keyName.ToUpper().Replace('.', '_'));

        private Tuple<string, string> GetConfiguration(string key)
        {
            var value = _configuration[Key(key)];

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return Tuple.Create(key, value);
        }

        public Dictionary<string, string> GetConfiguration()
        {
            var configurationKeys = new[]
            {
                "group.id",
                "enable.auto.commit",
                "bootstrap.servers",
                "broker.version.fallback",
                "api.version.fallback.ms",
                "ssl.ca.location",
                "sasl.username",
                "sasl.password",
                "sasl.mechanisms",
                "security.protocol",
            };

            //config.Add(new KeyValuePair<string, object>("request.timeout.ms", "3000"));

            return configurationKeys
                .Select(key => GetConfiguration(key))
                .Where(pair => pair != null)
                .ToDictionary(pair => pair.Item1, pair => pair.Item2);
        }
    }
}