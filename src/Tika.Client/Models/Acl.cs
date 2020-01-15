namespace Tika.Client
{
    public class Acl
    {
        public long ServiceAccountId { get; set; }
        public bool Allow { get; set; }
        public string Operation { get; set; }
        public string TopicPrefix { get; set; }
        public string ConsumerGroupPrefix { get; set; }
    }
}