using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.FakeTikaRestClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Xunit;
using Topic = KafkaJanitor.RestApi.FakeTikaRestClient.Models.Topic;

namespace KafkaJanitor.IntegrationTests
{
    public class TopicCreatedScenario
    {
        private readonly ITikaRestClient _tikaRestClient;

        // In a weird state until the new code is moved in from branch 'tika-client'
        [Fact]
        public async Task TopicCreatedScenarioRecipe()
        {
            await When_a_topic_creation_is_requested();
            await Then_a_topic_is_created();
            var topic = await Then_the_topic_is_retrieved();
        }

        private async Task When_a_topic_creation_is_requested()
        {
            var message = new
            {
                TopicName = "devex-integrationtest"
            };
            await _tikaRestClient.AddAsync(message.TopicName);
        }

        private async Task Then_a_topic_is_created()
        {
            Thread.Sleep(5 * 1000);
            var results = _tikaRestClient.GetAllAsync().Result.Single(t => t.Name == "devex-integrationtest");
        }

        private async Task<Topic> Then_the_topic_is_retrieved()
        {
            var topic = _tikaRestClient.GetAllAsync().Result.Single(t => t.Name == "devex-integrationtest");

            return topic;
        }
        
        public TopicCreatedScenario()
        {
            var conf = new ConfigurationBuilder()
                .Add(new EnvironmentVariablesConfigurationSource())
                .Build();
            var kafkaClient = new FakeTikaRestClient();

        }
    }
}