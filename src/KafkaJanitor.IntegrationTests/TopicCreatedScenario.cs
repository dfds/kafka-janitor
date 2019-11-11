using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaJanitor.IntegrationTests.Utils;
using KafkaJanitor.WebApp.Infrastructure.Messaging;
using KafkaJanitor.WebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Xunit;

namespace KafkaJanitor.IntegrationTests
{
    public class TopicCreatedScenario
    {
        private readonly ITopicRepository _topicRepository;

        public async Task TopicCreatedScenarioRecipe()
        {
            await When_a_topic_creation_is_requested();
            await Then_a_topic_is_created();
            var topic = await Then_the_topic_is_retrieved();
        }

        private async Task When_a_topic_creation_is_requested()
        {
            var kafkaClient = new KafkaClient();

            var message = new
            {
                TopicName = "devex-integrationtest"
            };
            await kafkaClient.SendMessageAsync(message);
        }

        private async Task Then_a_topic_is_created()
        {
            Thread.Sleep(5 * 1000);
            var exists = await _topicRepository.Exists("devex-integrationtest");
            Assert.True(exists);
        }

        private async Task<WebApp.Models.Topic> Then_the_topic_is_retrieved()
        {
            var topic = _topicRepository.GetAll().Result.First(t => t.Name.Equals("devex-integrationtest"));

            return topic;
        }
        
        public TopicCreatedScenario()
        {
            var conf = new ConfigurationBuilder()
                .Add(new EnvironmentVariablesConfigurationSource())
                .Build();
            _topicRepository = new TopicRepository(new KafkaConfiguration(conf));
        }
    }
}