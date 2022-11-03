using KafkaJanitor.App.Infrastructure.ConfluentCloud;
using KafkaJanitor.Tests.TestDoubles;

namespace KafkaJanitor.Tests.Builders;

public class JsonBasedConfluentCredentialsProviderBuilder
{
    private ITextContentReader _reader;

    public JsonBasedConfluentCredentialsProviderBuilder()
    {
        _reader = StubTextContentReader.AsEmpty();
    }

    public JsonBasedConfluentCredentialsProviderBuilder WithReader(ITextContentReader reader)
    {
        _reader = reader;
        return this;
    }

    public JsonBasedConfluentCredentialsProviderBuilder WithContent(params string[] lines)
    {
        _reader = StubTextContentReader.Containing(lines);
        return this;
    }

    public JsonBasedConfluentCredentialsProvider Build()
    {
        return new JsonBasedConfluentCredentialsProvider(_reader);
    }

    public static implicit operator JsonBasedConfluentCredentialsProvider(JsonBasedConfluentCredentialsProviderBuilder builder)
        => builder.Build();
}