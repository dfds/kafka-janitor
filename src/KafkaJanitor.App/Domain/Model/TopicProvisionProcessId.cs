namespace KafkaJanitor.App.Domain.Model;

public class TopicProvisionProcessId : ValueObject
{
    public static readonly TopicProvisionProcessId None = new TopicProvisionProcessId(Guid.Empty);
    private readonly Guid _value;

    private TopicProvisionProcessId(Guid value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString()
    {
        return _value.ToString("N");
    }

    public static TopicProvisionProcessId Parse(string? text)
    {
        if (TryParse(text, out var value))
        {
            return value;
        }

        throw new FormatException($"Value \"{text}\" is not a valid topic provisioning id.");
    }

    public static bool TryParse(string? text, out TopicProvisionProcessId processId)
    {
        if (Guid.TryParse(text, out var result))
        {
            processId = new TopicProvisionProcessId(result);
            return true;
        }

        processId = null!;
        return false;
    }

    public static TopicProvisionProcessId New()
        => new TopicProvisionProcessId(Guid.NewGuid());
}