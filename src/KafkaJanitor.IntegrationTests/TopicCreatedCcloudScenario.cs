using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using Tika.Client;
using Xunit;

namespace KafkaJanitor.IntegrationTests
{
    public class TopicCreatedCcloudScenario
    {
        private IConfiguration _configuration;
        
        [Fact]
        public async Task TopicCreatedCcloudScenarioRecipe()
        {
            var topic = await When_a_topic_is_requested();
            await Then_a_topic_is_created(topic);
            await Then_the_topic_is_retrieved(topic.Name);
        }

        private async Task<Topic> When_a_topic_is_requested()
        {
            var topic = new Topic("devex-integrationtest2", 3);

            return topic;
        }

        private async Task Then_a_topic_is_created(Topic topic)
        {
            var tikaClient = new TikaClient(new HttpClient(), Options.Create(new TikaOptions(_configuration)));
            await tikaClient.CreateTopic(topic.Name, topic.Partitions.ToString());
        }

        private async Task Then_the_topic_is_retrieved(string topicName)
        {
            var tikaClient = new TikaClient(new HttpClient(), Options.Create(new TikaOptions(_configuration)));
            var topics = await tikaClient.GetTopics();
            topics.First(to => to.Equals(topicName));
        }

        public TopicCreatedCcloudScenario()
        {
            _configuration = new ConfigurationBuilder()
                .Add(new EnvironmentVariablesConfigurationSource())
                .Build();
        }
    }
}