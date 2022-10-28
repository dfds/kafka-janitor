using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class TopicNameBuilder
{
    private string _capabilityRootId;
    private string _name;
    private bool _isPublic;

    public TopicNameBuilder()
    {
        _capabilityRootId = "foo";
        _name = "bar";
        _isPublic = true;
    }

    public TopicNameBuilder WithCapabilityRootId(string capabilityRootId)
    {
        _capabilityRootId = capabilityRootId;
        return this;
    }

    public TopicNameBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TopicNameBuilder WithIsPublic(bool isPublic)
    {
        _isPublic = isPublic;
        return this;
    }

    public TopicName Build()
    {
        return new TopicName(
            capabilityRootId: CapabilityRootId.Parse(_capabilityRootId),
            name: _name,
            isPublic: _isPublic
        );
    }
}