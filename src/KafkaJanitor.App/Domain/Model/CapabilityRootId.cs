namespace KafkaJanitor.App.Domain.Model;

public class CapabilityRootId : ValueObject
{
    private readonly string _value;

    private CapabilityRootId(string value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString() => _value;

    public static CapabilityRootId Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new FormatException($"Value \"{text}\" is not a valid capability root id.");
        }

        return new CapabilityRootId(text);
    }

    public static bool TryParse(string? text, out CapabilityRootId rootId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            rootId = null!;
            return false;
        }

        rootId = new CapabilityRootId(text);
        return true;
    }

    // NOTE [jandr@2022-10-14]: dont actually know about this one - good/bad idea?!
    public static implicit operator string(CapabilityRootId rootId) 
        => rootId.ToString();
}