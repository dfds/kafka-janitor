namespace KafkaJanitor.WebApp.Models
{
    public class ApiKey
    {
        public string Id { get; set; }
        public string ServiceAccountId { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
    }
}