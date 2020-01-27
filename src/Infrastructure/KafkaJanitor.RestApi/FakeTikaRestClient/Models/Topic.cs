namespace KafkaJanitor.RestApi.FakeTikaRestClient.Models
{
    public class Topic
    {
        public Topic(string name, int partitions)
        {
            Name = name;
            Partitions = partitions;
        }
        
        public string Name { get; }
        public int Partitions { get; }
    }
}