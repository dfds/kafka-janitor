namespace KafkaJanitor.App.Domain.Model;

public class ClusterAccessId : ValueObject
{
    private readonly Guid _value;

    private ClusterAccessId(Guid value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString()
        => _value.ToString();

    public static ClusterAccessId Parse(string? text)
    {
        if (TryParse(text, out var id))
        {
            return id;
        }

        throw new FormatException($"Value \"{text}\" is not a valid cluster access id");
    }

    public static bool TryParse(string? text, out ClusterAccessId id)
    {
        if (Guid.TryParse(text, out var value))
        {
            id = new ClusterAccessId(value);
            return true;
        }

        id = null!;
        return false;
    }

    public static ClusterAccessId New() => new ClusterAccessId(Guid.NewGuid());

    public static implicit operator Guid(ClusterAccessId id)
        => id._value;

    public static implicit operator ClusterAccessId(Guid id)
        => new ClusterAccessId(id);
}