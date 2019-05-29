namespace KafkaJanitor.WebApp.Models
{
    public class Topic
    {
        private const int DefaultPartitionCount = 12;

        public Topic(string name, int partitions = DefaultPartitionCount)
        {
            Name = name;
            Partitions = partitions;
        }
        
        public string Name { get; }
        public int Partitions { get; }
    }
}