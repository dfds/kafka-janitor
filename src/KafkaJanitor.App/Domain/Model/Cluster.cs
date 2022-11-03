namespace KafkaJanitor.App.Domain.Model;

public class Cluster : AggregateRoot<ClusterId>
{
    public Cluster(ClusterId id, string name, string bootstrapEndpoint, string adminApiEndpoint) : base(id)
    {
        Name = name;
        BootstrapEndpoint = bootstrapEndpoint;
        AdminApiEndpoint = adminApiEndpoint;
    }

    public string Name { get; private set; }
    public string BootstrapEndpoint { get; private set; }
    public string AdminApiEndpoint { get; private set; }

    public override string ToString()
    {
        return Id.ToString();
    }
}