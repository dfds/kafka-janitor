namespace KafkaJanitor.App.Domain.Model;

public class ClusterAccessDefinitionId : ValueObject
{
    private readonly Guid _value;

    private ClusterAccessDefinitionId(Guid value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString()
        => _value.ToString();

    public static ClusterAccessDefinitionId Parse(string? text)
    {
        if (TryParse(text, out var id))
        {
            return id;
        }

        throw new FormatException($"Value \"{text}\" is not a valid cluster access id");
    }

    public static bool TryParse(string? text, out ClusterAccessDefinitionId id)
    {
        if (Guid.TryParse(text, out var value))
        {
            id = new ClusterAccessDefinitionId(value);
            return true;
        }

        id = null!;
        return false;
    }

    public static ClusterAccessDefinitionId New() => new ClusterAccessDefinitionId(Guid.NewGuid());

    public static implicit operator Guid(ClusterAccessDefinitionId id)
        => id._value;

    public static implicit operator ClusterAccessDefinitionId(Guid id)
        => new ClusterAccessDefinitionId(id);
}