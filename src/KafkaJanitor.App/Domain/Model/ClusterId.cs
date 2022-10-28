namespace KafkaJanitor.App.Domain.Model;

public class ClusterId : ValueObject
{
    private const string ValueOfNone = ":none:";
    public static readonly ClusterId None = new ClusterId(ValueOfNone);

    private readonly string _value;

    private ClusterId(string value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString()
    {
        return _value;
    }

    public static ClusterId Parse(string? text)
    {
        if (TryParse(text, out var result))
        {
            return result;
        }

        throw new FormatException($"Value \"{text}\" is not a valid cluster id.");
    }

    public static bool TryParse(string? text, out ClusterId clusterId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            clusterId = null!;
            return false;
        }

        if (ValueOfNone.Equals(text, StringComparison.InvariantCultureIgnoreCase))
        {
            clusterId = None;
        }
        else
        {
            clusterId = new ClusterId(text);
        }

        return true;
    }
}