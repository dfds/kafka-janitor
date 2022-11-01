namespace KafkaJanitor.App.Domain.Model;

public class ClusterApiKeyId : ValueObject
{
    private readonly Guid _value;

    private ClusterApiKeyId(Guid value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString() 
        => _value.ToString();

    public static ClusterApiKeyId Parse(string? text)
    {
        if (TryParse(text, out var id))
        {
            return id;
        }

        throw new FormatException($"Value \"{text}\" is not a valid api key id");
    }

    public static bool TryParse(string? text, out ClusterApiKeyId id)
    {
        if (Guid.TryParse(text, out var value))
        {
            id = new ClusterApiKeyId(value);
            return true;
        }

        id = null!;
        return false;
    }

    public static ClusterApiKeyId New() => new ClusterApiKeyId(Guid.NewGuid());

    public static implicit operator Guid(ClusterApiKeyId id)
        => id._value;

    public static implicit operator ClusterApiKeyId(Guid id)
        => new ClusterApiKeyId(id);
}