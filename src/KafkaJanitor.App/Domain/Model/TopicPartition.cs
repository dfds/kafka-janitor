namespace KafkaJanitor.App.Domain.Model;

public class TopicPartition : ValueObject
{
    private readonly uint _value;

    private TopicPartition(uint value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }

    public uint ToNumber() => _value;

    public static TopicPartition From(uint partitionCount)
    {
        if (partitionCount == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(partitionCount), partitionCount, "Partition count must be greater that zero.");
        }

        return new TopicPartition(partitionCount);
    }

    public static bool TryParse(string? text, out TopicPartition partition)
    {
        if (uint.TryParse(text, out var value) && value > 0)
        {
            partition = new TopicPartition(value);
            return true;
        }

        partition = null!;
        return false;
    }
}