using Microsoft.Extensions.Configuration;

namespace KafkaJanitor.RestClient
{
    public class ClientOptions
    {
        public string KAFKAJANITOR_API_ENDPOINT { get; set; }
        
        public ClientOptions() {}

        public ClientOptions(IConfiguration conf)
        {
            KAFKAJANITOR_API_ENDPOINT = conf["KAFKAJANITOR_API_ENDPOINT"];
        }
    }
}