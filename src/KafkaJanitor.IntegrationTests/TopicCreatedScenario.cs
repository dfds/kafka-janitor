using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using Tika.RestClient;
using Tika.RestClient.Factories;
using Tika.RestClient.Features.Topics.Models;
using Xunit;

namespace KafkaJanitor.IntegrationTests
{
    public class TopicCreatedScenario
    {
        private readonly IRestClient _tikaRestClient;

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
            await _tikaRestClient.Topics.CreateAsync(new TopicCreate
            {
                name = message.TopicName
            });
        }

        private async Task Then_a_topic_is_created()
        {
            Thread.Sleep(5 * 1000);
            var results = _tikaRestClient.Topics.GetAllAsync().Result.Single(t => t == "devex-integrationtest");
        }

        private async Task<Topic> Then_the_topic_is_retrieved()
        {
            var topic = _tikaRestClient.Topics.GetAllAsync().Result.Single(t => t == "devex-integrationtest");

            return new Topic
            {
                Name = topic
            };
        }
        
        public TopicCreatedScenario()
        {
            var conf = new ConfigurationBuilder()
                .Add(new EnvironmentVariablesConfigurationSource())
                .Build();
            var options = Options.Create(new ClientOptions(conf));
            _tikaRestClient = RestClientFactory.CreateFromConfiguration(new HttpClient(), options);
        }
    }
}