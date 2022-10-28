namespace KafkaJanitor.App.Domain.Model;

public class Topic
{
    public Topic(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
}