using Confluent.Kafka;

namespace KafkaClient
{
    public class ConsumerFactory
    {
        public static JsonConsumer CreateJsonConsumer(
            string key,
            string secret
        )
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "pkc-e8wrm.eu-central-1.aws.confluent.cloud:9092",
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = key,
                SaslPassword = secret
            };

            var builder = new ConsumerBuilder<string, string>(config);
            var jsonProducer = new JsonConsumer(builder);

            return jsonProducer;
        }
    }
}