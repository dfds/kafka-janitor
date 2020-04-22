using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace KafkaClient
{
    public class JsonConsumer
    {
        private ConsumerBuilder<string, string> _consumerBuilder;

        public JsonConsumer(ConsumerBuilder<string,string> consumerBuilder)
        {
            _consumerBuilder = consumerBuilder;
        }

        public T ConsumeOne<T>(string topicName)
        {
            using (var consumer = _consumerBuilder.Build())
            {
                consumer.Subscribe(topicName);

                var consumeResult = consumer.Consume();

                var result = JsonConvert.DeserializeObject<T>(consumeResult.Message.Value);

                consumer.Commit(consumeResult);
                
                return result;
            }
        }
    }
}