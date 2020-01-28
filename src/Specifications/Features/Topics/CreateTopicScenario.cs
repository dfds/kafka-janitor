using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestClient.Factories;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Specifications.Features.Topics
{
    public class CreateTopicScenario
    {
        private IHost testHost;
        private IRestClient restClient;
        private string topicName;
        
        [Fact]
        public async Task CreateTopicRecipe()
        {
            await Given_a_rest_api();
                  And_a_rest_client();
            await When_a_topic_is_created();
            await Then_it_can_be_found_in_the_topic_list();
        }

        private async Task Given_a_rest_api()
        {
            testHost = await RestApiHostCreator.CreateAsync();
        }

        private void And_a_rest_client()
        {
            var testHttpClient = HostBuilderTestServerExtensions.GetTestClient(testHost);

            restClient = RestClientFactory.Create(testHttpClient);
        }

        private async Task When_a_topic_is_created()
        {
            topicName = $"testTopic_{DateTime.Now:O}";

            var newTopic = new Topic
            {
                Name = topicName, 
                Description = "None", 
                Partitions = 3
            };
            
            await restClient.Topics.CreateAsync(newTopic);
        }

        private async Task Then_it_can_be_found_in_the_topic_list()
        {
            var topics = await restClient.Topics.GetAllAsync();

            topics.Single(t => t.Name == topicName);
        }
    }
}