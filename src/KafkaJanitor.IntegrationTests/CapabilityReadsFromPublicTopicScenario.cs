using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KafkaClient;
using KafkaJanitor.RestApi.Features.Vault.Model;
using KafkaJanitor.RestClient.Features.Access.Models;
using Xunit;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Topic = KafkaJanitor.RestClient.Features.Topics.Models.Topic;

namespace KafkaJanitor.IntegrationTests
{
    public class CapabilityReadsFromPublicTopicScenario : IDisposable
    {
        private Topic _publicTopic;

        private IHost testHost;
        private readonly RestClient.IRestClient _kafkaClient;
        private readonly HttpClient _kafkaJanitorHttpClient;
        private string _random5chars;
        private string _producerCapabilityRootId;
        private string _consumerCapabilityRootId;
        private Dictionary<string, List<ApiCredentials>> _allKeySecrets;
        private TestPayload _testMessage;

        [Fact]
        public async Task CapabilityReadsFromPublicTopicRecipe()
        {
            await Given_producer_access();
            await And_a_public_topic();
            await And_consumer_access();
            await When_a_message_is_published_in_the_public_topic();
                  Then_the_message_can_be_consumed_by_a_other_capability();
        }

        private async Task Given_producer_access()
        {
            _producerCapabilityRootId = "test-public-topic-producer" + _random5chars;
            var accountRequest = new ServiceAccountRequestInput
            {
                CapabilityId = Guid.NewGuid().ToString(),
                CapabilityName = _producerCapabilityRootId,
                TopicPrefix = _producerCapabilityRootId + ".",
                CapabilityRootId = _producerCapabilityRootId
            };

            await _kafkaClient.Access.RequestAsync(accountRequest);
        }

        private async Task And_a_public_topic()
        {
            _random5chars = Guid.NewGuid().ToString().Substring(0, 5);
            _publicTopic = new Topic
            {
                Name = "pub.devex-integrationtest" + _random5chars,
                Partitions = 1
            };

            await _kafkaClient.Topics.CreateAsync(_publicTopic);
        }


        private async Task And_consumer_access()
        {
            _consumerCapabilityRootId = "test-public-topic-" + _random5chars;
            var accountRequest = new ServiceAccountRequestInput
            {
                CapabilityId = Guid.NewGuid().ToString(),
                CapabilityName = _consumerCapabilityRootId,
                TopicPrefix = _consumerCapabilityRootId + ".",
                CapabilityRootId = _consumerCapabilityRootId
            };

            await _kafkaClient.Access.RequestAsync(accountRequest);
        }

        private async Task When_a_message_is_published_in_the_public_topic()
        {
            var getKeySecretsResponse = await _kafkaJanitorHttpClient.GetAsync(new Uri("api/vault", UriKind.Relative));
            getKeySecretsResponse.EnsureSuccessStatusCode();

            var content = await getKeySecretsResponse.Content.ReadAsStringAsync();
            _allKeySecrets = JsonConvert.DeserializeObject<Dictionary<string, List<ApiCredentials>>>(content);
            var producerKeySecrets = _allKeySecrets[_producerCapabilityRootId];

            var jsonProducer = ProducerFactory.CreateJsonProducer(
                producerKeySecrets.Single().Key,
                producerKeySecrets.Single().Secret
            );

            _testMessage = new TestPayload {Message = "All is well " + DateTime.UtcNow.ToString("u")};
            await jsonProducer.ProduceAsync(
                _publicTopic.Name,
                _random5chars,
                _testMessage
            );
        }
     
        private void Then_the_message_can_be_consumed_by_a_other_capability()
        {
            var consumerKeySecrets = _allKeySecrets[_producerCapabilityRootId];

            var jsonConsumer = ConsumerFactory.CreateJsonConsumer(
                consumerKeySecrets.Single().Key,
                consumerKeySecrets.Single().Secret
            );

            var resultMessage = jsonConsumer.ConsumeOne<TestPayload>(_publicTopic.Name);
            Assert.Equal(_testMessage, resultMessage);
        }
        
        public class TestPayload
        {
            public string Message { get; set; }
        }

        public CapabilityReadsFromPublicTopicScenario()
        {
            _kafkaJanitorHttpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };


            _kafkaClient = RestClient.Factories.RestClientFactory.Create(_kafkaJanitorHttpClient);
        }

        public void Dispose()
        {
            _kafkaClient.Topics.DeleteAsync(_publicTopic.Name).Wait();

            // delete permission stuff?
        }
    }
}