using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KafkaClient
{
    public class JsonProducer
    {
        private ProducerBuilder<string, string> _producerBuilder;
        public JsonProducer(ProducerBuilder<string, string> producerBuilder)
        {
            _producerBuilder = producerBuilder;
        }
        public async Task ProduceAsync(
            string topicName,
            string key,
            object message
        )
        {
            var payload = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            using (var producer = _producerBuilder.Build())
            {
                await producer.ProduceAsync(
                    topicName, new Message<string, string>
                    {
                        Key = key,
                        Value = payload
                    });
            }
        }
    }
}