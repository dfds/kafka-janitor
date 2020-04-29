using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KafkaClient;
using KafkaJanitor.IntegrationTests.Utils;
using KafkaJanitor.RestApi.Features.Vault.Model;
using KafkaJanitor.RestClient.Features.Access.Models;
using Xunit;
using Newtonsoft.Json;
using Topic = KafkaJanitor.RestClient.Features.Topics.Models.Topic;

namespace KafkaJanitor.IntegrationTests
{
    public class CapabilityReadsFromPublicTopicScenario : IDisposable
    {
        private Topic _publicTopic;

        private readonly RestClient.IRestClient _kafkaClient;
        private readonly HttpClient _kafkaJanitorHttpClient;

        private string _random5chars;

        private TestPayload _testMessage;

        private string _producerCapabilityRootId;
        private ApiCredentials _producerKeySecret;

        private string _consumerCapabilityRootId;
        private string _consumerGroupId;
        private ApiCredentials _consumerKeySecret;
        private JsonConsumer _jsonConsumer;

        [Fact]
        public async Task CapabilityReadsFromPublicTopicRecipe()
        {
            await Given_producer_access();
            await And_consumer_access();

            await And_a_public_topic();
            await And_a_consumer();
            Thread.Sleep(120000); // allow ccloud to do its thing, see if we cna remove this in the future

            await When_a_message_is_published_in_the_public_topic();

            Then_the_message_can_be_consumed_by_a_other_capability();
        }

        private async Task Given_producer_access()
        {
            _random5chars = Guid.NewGuid().ToString().Substring(0, 5);
            _producerCapabilityRootId = "test-public-topic-producer-" + _random5chars;

            var accountRequest = new ServiceAccountRequestInput
            {
                CapabilityId = Guid.NewGuid().ToString(),
                CapabilityName = _producerCapabilityRootId,
                TopicPrefix = _producerCapabilityRootId,
                CapabilityRootId = _producerCapabilityRootId
            };

            await _kafkaClient.Access.RequestAsync(accountRequest);

            var getKeySecretsResponse = await _kafkaJanitorHttpClient.GetAsync(new Uri("api/vault", UriKind.Relative));
            getKeySecretsResponse.EnsureSuccessStatusCode();

            var content = await getKeySecretsResponse.Content.ReadAsStringAsync();
            var allKeySecrets = JsonConvert.DeserializeObject<Dictionary<string, List<ApiCredentials>>>(content);
            _producerKeySecret = allKeySecrets[_producerCapabilityRootId].Single();
        }

        private async Task And_a_public_topic()
        {
            _publicTopic = new Topic
            {
                Name = "pub." + _producerCapabilityRootId + ".delete-me",
                Partitions = 1
            };
            await _kafkaClient.Topics.CreateAsync(_publicTopic);
        }


        private async Task And_consumer_access()
        {
            var consumerStart = "test-public-topic-consume";
            _consumerCapabilityRootId = consumerStart + _random5chars;
            _consumerGroupId = consumerStart + _random5chars + ".";

            var accountRequest = new ServiceAccountRequestInput
            {
                CapabilityId = Guid.NewGuid().ToString(),
                CapabilityName = _consumerCapabilityRootId,
                TopicPrefix = _consumerCapabilityRootId,
                CapabilityRootId = _consumerCapabilityRootId
            };

            await _kafkaClient.Access.RequestAsync(accountRequest);

            var getKeySecretsResponse = await _kafkaJanitorHttpClient.GetAsync(new Uri("api/vault", UriKind.Relative));
            getKeySecretsResponse.EnsureSuccessStatusCode();

            var content = await getKeySecretsResponse.Content.ReadAsStringAsync();
            var allKeySecrets = JsonConvert.DeserializeObject<Dictionary<string, List<ApiCredentials>>>(content);
            _consumerKeySecret = allKeySecrets[_consumerCapabilityRootId].Single();
        }

        private async Task And_a_consumer()
        {
            _jsonConsumer = ConsumerFactory.CreateJsonConsumer(
                _consumerKeySecret.Key,
                _consumerKeySecret.Secret,
                _consumerGroupId
            );
            _jsonConsumer.StartConsuming(_publicTopic.Name);
        }

        private async Task When_a_message_is_published_in_the_public_topic()
        {
            var jsonProducer = ProducerFactory.CreateJsonProducer(
                _producerKeySecret.Key,
                _producerKeySecret.Secret
            );

            _testMessage = new TestPayload {Message = "All is well " + DateTime.UtcNow.ToString("u")};

            await jsonProducer.ProduceAsync(
                _publicTopic.Name,
                Guid.NewGuid().ToString(),
                _testMessage
            );
        }

        private void Then_the_message_can_be_consumed_by_a_other_capability()
        {
            var payload = DoUntil.ResultOrTimespan(
                () =>
                {
                    if (_jsonConsumer.ConsumedMessages.Any() == false)
                    {
                        return null;
                    }

                    var t = _jsonConsumer.ConsumeOneOrDefault<TestPayload>();

                    return t;
                },
                TimeSpan.FromSeconds(5)
            );
            Assert.NotNull(payload);
            //     Assert.Equal(_testMessage.Message, payload.Message);
        }

        private class TestPayload
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
            _jsonConsumer.Dispose();
            // delete permission stuff?
        }
    }
}