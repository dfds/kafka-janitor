namespace KafkaJanitor.App.Domain.Model;

public class GrantCapabilityAccessProcessId : ValueObject
{
    private readonly Guid _value;

    private GrantCapabilityAccessProcessId(Guid value)
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

    public static GrantCapabilityAccessProcessId Parse(string? text)
    {
        if (TryParse(text, out var value))
        {
            return value;
        }

        throw new FormatException($"Value \"{text}\" is not a valid grant capability access process id.");
    }

    public static bool TryParse(string? text, out GrantCapabilityAccessProcessId processId)
    {
        if (Guid.TryParse(text, out var result))
        {
            processId = new GrantCapabilityAccessProcessId(result);
            return true;
        }

        processId = null!;
        return false;
    }

    public static GrantCapabilityAccessProcessId New()
        => new GrantCapabilityAccessProcessId(Guid.NewGuid());
}