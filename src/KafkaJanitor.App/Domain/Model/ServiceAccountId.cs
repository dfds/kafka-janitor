namespace KafkaJanitor.App.Domain.Model;

public class ServiceAccountId : ValueObject
{
    private readonly string _value;

    private ServiceAccountId(string value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString() => _value;

    public static ServiceAccountId Parse(string? text)
    {
        if (!TryParse(text, out var id))
        {
            throw new FormatException($"Value \"{text}\" is not a valid service account id.");
        }

        return id;
    }

    public static bool TryParse(string? text, out ServiceAccountId id)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            id = null!;
            return false;
        }

        var idPrefix = "user:";

        if (text.StartsWith(idPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            text = text.Substring(idPrefix.Length);
        }

        id = new ServiceAccountId(text);
        return true;
    }
}