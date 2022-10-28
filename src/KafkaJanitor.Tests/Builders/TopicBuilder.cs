using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class TopicBuilder
{
    private string _name;

    public TopicBuilder()
    {
        _name = "foo";
    }

    public TopicBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public Topic Build()
    {
        return new Topic(_name);
    }
}