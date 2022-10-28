namespace KafkaJanitor.App.Domain.Model;

public class Cluster : AggregateRoot<ClusterId>
{
    public Cluster(ClusterId id, string name, string bootstrapEndpoint) : base(id)
    {
        Name = name;
        BootstrapEndpoint = bootstrapEndpoint;
    }

    public string Name { get; private set; }
    public string BootstrapEndpoint { get; private set; }
}