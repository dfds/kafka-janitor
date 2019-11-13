using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Infrastructure.Messaging;
using KafkaJanitor.WebApp.Models;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace KafkaJanitor.Tests
{
    public class Topic
    {
        private readonly ITopicRepository _topicRepository;
        
        public Topic()
        {
            var conf = new ConfigurationBuilder()
                .Add(new EnvironmentVariablesConfigurationSource())
                .Build();
            _topicRepository = new TopicRepository(new KafkaConfiguration(conf));
        }
        
        [Fact]
        async Task CanCreateTopicAndRetrieveIt()
        {
            const string topicName = "devex-integrationtest";
            const int topicPartitions = 3;
            await _topicRepository.Add(new WebApp.Models.Topic(topicName, topicPartitions));

            var topics = await _topicRepository.GetAll();
            var createdTopic = topics.First(t => t.Name.Equals(topicName));
            
            Assert.Equal(topicName, createdTopic.Name);
            Assert.Equal(topicPartitions, createdTopic.Partitions);
        }
    }
}