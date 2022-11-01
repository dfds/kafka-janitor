namespace KafkaJanitor.App.Domain.Model;

public class TopicRetention : ValueObject
{
    public static readonly TopicRetention Infinite = new TopicRetention(0);
    private readonly uint _value;

    public TopicRetention(uint value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString() => TimeSpan
        .FromMilliseconds(_value)
        .ToString();

    public uint ToMilliseconds() => _value;

    public static TopicRetention FromMilliseconds(uint milliseconds) => milliseconds == 0
        ? Infinite
        : new TopicRetention(milliseconds);

    public static TopicRetention Parse(string? text)
    {
        if (TryParse(text, out var value))
        {
            return value;
        }

        throw new FormatException($"Value \"{text}\" is not a valid topic retention.");
    }

    public static bool TryParse(string? text, out TopicRetention retention)
    {
        if (uint.TryParse(text, out var value))
        {
            retention = FromMilliseconds(value);
            return true;
        }

        retention = null!;
        return false;
    }
}