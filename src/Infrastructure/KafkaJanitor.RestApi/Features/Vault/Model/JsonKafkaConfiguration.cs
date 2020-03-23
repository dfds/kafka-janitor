using Newtonsoft.Json;

namespace KafkaJanitor.RestApi.Features.Vault.Model
{
    public class JsonKafkaConfiguration
    {
        [JsonProperty("bootstrap.servers")] public string BootstrapServers { get; set; }
        [JsonProperty("security.protocol")] public string SecurityProtocol { get; set; }
        [JsonProperty("sasl.mechanism")] public string SaslMechanism { get; set; }
        [JsonProperty("sasl.username")] public string SaslUsername { get; set; }
        [JsonProperty("sasl.password")] public string SaslPassword { get; set; }


        public static JsonKafkaConfiguration Create(
            string username,
            string password
        )
        {
            var kafkaConfiguration = new JsonKafkaConfiguration
            {
                BootstrapServers = "pkc-e8wrm.eu-central-1.aws.confluent.cloud:9092",
                SecurityProtocol = "SASL_SSL",
                SaslMechanism = "PLAIN",
                SaslUsername = username,
                SaslPassword = password
            };

            return kafkaConfiguration;
        }
    }
}