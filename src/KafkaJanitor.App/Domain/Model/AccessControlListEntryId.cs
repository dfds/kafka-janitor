namespace KafkaJanitor.App.Domain.Model;

public class AccessControlListEntryId : ValueObject
{
    private readonly Guid _value;

    private AccessControlListEntryId(Guid value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString() 
        => _value.ToString("N");

    public static AccessControlListEntryId Parse(string? text)
    {
        if (TryParse(text, out var value))
        {
            return value;
        }

        throw new FormatException($"Value \"{text}\" is not a valid access control list entry id.");
    }

    public static bool TryParse(string? text, out AccessControlListEntryId id)
    {
        if (Guid.TryParse(text, out var result))
        {
            id = new AccessControlListEntryId(result);
            return true;
        }

        id = null!;
        return false;
    }

    public static AccessControlListEntryId New()
        => new AccessControlListEntryId(Guid.NewGuid());
}