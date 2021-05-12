using System.Collections.Generic;

namespace KafkaJanitor.RestClient.Features.Topics.Models
{
    public class Topic
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClusterId { get; set; }
        public int Partitions { get; set; }
        public Dictionary<string, object> Configurations { get; set; } = new Dictionary<string, object>();
    }
}