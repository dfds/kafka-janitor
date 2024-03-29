namespace KafkaJanitor.RestClient.Features.Access.Models
{
    public class ServiceAccountRequestInput
    {
        public string CapabilityName { get; set; }
        public string CapabilityId { get; set; }
        public string CapabilityRootId { get; set; }
        public string TopicPrefix { get; set; }
        public string ClusterId { get; set; }
    }
}