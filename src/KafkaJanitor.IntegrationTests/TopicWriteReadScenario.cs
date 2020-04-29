using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaClient;
using KafkaJanitor.IntegrationTests.Utils;
using Xunit;

namespace KafkaJanitor.IntegrationTests
{
    public class TopicWriteReadScenario : IDisposable
    {

        private string _random5chars;
        private TestPayload _testMessage;
        private string _key;
        private string _secret;
        private string _topicName = "pub.test-public-topic-producer-4ddf2.delete-me";
        private JsonConsumer _jsonConsumer;

        [Fact(Skip="Us it for debugging producer/consumer")]
        public async Task TopicWriteReadRecipe()
        {
            await When_a_message_is_published_in_the_public_topic();
            And_a_consumer_is_added_to_topic();
            Then_the_message_can_be_consumed_by_a_other_capability();
        }

        private async Task When_a_message_is_published_in_the_public_topic()
        {
            var jsonProducer = ProducerFactory.CreateJsonProducer(
                "producerKey",
                "producerSecret"
            );

            _testMessage = new TestPayload {Message = "All is well " + DateTime.UtcNow.ToString("u")};

            await jsonProducer.ProduceAsync(
                _topicName,
                _random5chars,
                _testMessage
            );
        }

        private void And_a_consumer_is_added_to_topic()
        {
            _jsonConsumer = ConsumerFactory.CreateJsonConsumer(
                "consumerKey",
                "consumerSecret",
                "consumerGroupId"
            );
            _jsonConsumer.StartConsuming(_topicName);
        }
        private void Then_the_message_can_be_consumed_by_a_other_capability()
        {
            var payload = DoUntil.ResultOrTimespan(
                () =>
                {
                    return _jsonConsumer.ConsumedMessages.Any() ? 
                        _jsonConsumer.ConsumeOneOrDefault<TestPayload>() : 
                        null;
                },
                TimeSpan.FromSeconds(5)
            );

            Assert.Equal(_testMessage.Message, payload.Message);
        }


        public class TestPayload
        {
            public string Message { get; set; }
        }

        public void Dispose()
        {
            _jsonConsumer.Dispose();
        }
    }
}