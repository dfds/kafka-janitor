using Confluent.Kafka;

namespace KafkaClient
{
    public static class ProducerFactory
    {
        public static JsonProducer CreateJsonProducer(
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

            var builder = new ProducerBuilder<string, string>(config);
            var jsonProducer = new JsonProducer(builder);

            return jsonProducer;
        }
    }
}