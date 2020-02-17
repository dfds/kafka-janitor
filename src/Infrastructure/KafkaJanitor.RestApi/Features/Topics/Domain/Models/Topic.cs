namespace KafkaJanitor.RestApi.Features.Topics.Domain.Models
{
    public class Topic
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Partitions { get; set; }
    }
}