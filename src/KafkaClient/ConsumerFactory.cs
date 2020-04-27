using Confluent.Kafka;

namespace KafkaClient
{
    public class ConsumerFactory
    {
        public static JsonConsumer CreateJsonConsumer(
            string key,
            string secret,
            string groupId
        )
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "pkc-e8wrm.eu-central-1.aws.confluent.cloud:9092",
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = key,
                SaslPassword = secret,
                GroupId = groupId
            };

            var builder = new ConsumerBuilder<string, string>(config);
            var jsonConsumer = new JsonConsumer(builder);

            return jsonConsumer;
        }
    }
}