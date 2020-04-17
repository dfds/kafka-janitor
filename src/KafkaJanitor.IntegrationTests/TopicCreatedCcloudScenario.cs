using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using Tika.RestClient;
using Tika.RestClient.Features.Topics.Models;
using Xunit;

namespace KafkaJanitor.IntegrationTests
{
    public class TopicCreatedCcloudScenario
    {
        private IRestClient _tikaClient;
        private Topic topic;
        [Fact]
        public async Task TopicCreatedCcloudScenarioRecipe()
        { 
            await When_a_topic_is_requested();
                  And_a_tikaClient_is_provided();
            await Then_a_topic_is_created(topic);
            await Then_the_topic_is_retrieved(topic.Name);
        }

        private void And_a_tikaClient_is_provided()
        {
            _tikaClient = Tika.RestClient.Factories.RestClientFactory.CreateFromConfiguration(new HttpClient(), Options.Create(new ClientOptions
            {
                TIKA_API_ENDPOINT = "http://localhost:3000/"
            }));
        }

        private async Task When_a_topic_is_requested()
        {
            topic = new Topic
            {
                Name = "devex-integrationtest2",
                Partitions = 3
            };
        }

        private async Task Then_a_topic_is_created(Topic topic)
        {
            var topicCreate = TopicCreate.Create(topic.Name, topic.Partitions);

            if (topic.Configurations != null)
            {
                foreach (var (key, value) in topic.Configurations)
                {
                    topicCreate = topicCreate.WithConfiguration(key, value);
                }
            }

            await _tikaClient.Topics.CreateAsync(topicCreate);
        }

        private async Task Then_the_topic_is_retrieved(string topicName)
        {
            var topics = await _tikaClient.Topics.GetAllAsync();
            topics.First(to => to.Equals(topicName));
        }

        public TopicCreatedCcloudScenario()
        {
        }
    }
}