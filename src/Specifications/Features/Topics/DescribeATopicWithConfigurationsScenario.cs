using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestClient;
using KafkaJanitor.RestClient.Factories;
using KafkaJanitor.RestClient.Features.Topics.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Specifications.TestDoubles;
using Xunit;

namespace Specifications.Features.Topics
{
    public class DescribeATopicWithConfigurationsScenario
    {
        private IHost testHost;
        private IRestClient restClient;
        private string topicName;
        private Topic _topicToCreate;

        [Fact]
        public async Task DescribeATopicWithConfigurationsRecipe()
        {
            await Given_a_rest_api_with_a_tika_spy();
            And_a_rest_client();
            await And_a_topic_with_configurations();
            await Then_the_topic_can_be_described();
        }

        private async Task Given_a_rest_api_with_a_tika_spy()
        {
            var builder = new RestApiHostBuilder()
                .WithService<Tika.RestClient.IRestClient>(new TikaRestClientSpy());

            testHost = await builder.CreateAsync();
        }

        private void And_a_rest_client()
        {
            var testHttpClient = HostBuilderTestServerExtensions.GetTestClient(testHost);

            restClient = RestClientFactory.Create(testHttpClient);
        }

        private async Task And_a_topic_with_configurations()
        {
            topicName = $"testTopic_{DateTime.Now:O}";

            _topicToCreate = new Topic
            {
                Name = topicName,
                Description = "None",
                Partitions = 3,
                Configurations =
                {
                    {"retention.ms", (long) -1},
                    {"unclean.leader.election.enable", false},
                    {"message.timestamp.type", "LogAppendTime"}
                }
            };

            await restClient.Topics.CreateAsync(_topicToCreate);
        }

        private async Task Then_the_topic_can_be_described()
        {
            var topicDescription = await restClient.Topics.DescribeAsync(topicName, null);


            foreach (var configuration in _topicToCreate.Configurations)
            {
                Assert.Equal(configuration.Value, topicDescription.Configurations[configuration.Key]);
            }
        }
    }
}