using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
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
        }

        private async Task When_a_topic_creation_is_requested()
        {
            var topicCreate = TopicCreate.Create("devex-integrationtest", 3);
            await _tikaRestClient.Topics.CreateAsync(topicCreate);
        }

        private async Task Then_a_topic_is_created()
        {
            _tikaRestClient
                .Topics
                .GetAllAsync()
                .Result
                .Single(t => t == "devex-integrationtest");
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