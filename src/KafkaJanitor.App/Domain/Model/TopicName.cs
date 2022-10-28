using System.Text.RegularExpressions;

namespace KafkaJanitor.App.Domain.Model;

public class TopicName : ValueObject
{
    private const string Separator = ".";

    public TopicName(CapabilityRootId capabilityRootId, string name, bool isPublic)
    {
        CapabilityRootId = capabilityRootId;
        Name = name;
        IsPublic = isPublic;
    }

    public CapabilityRootId CapabilityRootId { get; }
    public string Name { get; }
    public bool IsPublic { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CapabilityRootId;
        yield return Name;
        yield return IsPublic;
    }

    public override string ToString()
    {
        var result = IsPublic
            ? string.Join(Separator, "pub", CapabilityRootId, Name)
            : string.Join(Separator, CapabilityRootId, Name);

        return result.ToLowerInvariant();
    }

    public static TopicName Parse(string? text)
    {
        if (TryParse(text, out var result))
        {
            return result;
        }

        throw new FormatException($"Value \"{text}\" is not a valid value for topic.");
    }

    public static bool TryParse(string? text, out TopicName topicName)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            topicName = null!;
            return false;
        }

        var match = Regex.Match(text!.ToLowerInvariant(), @"^(?<availability>\w+\.)?(?<rootid>.*?)\.(?<name>.*?)$");

        var availability = match.Groups["availability"].Value;
        var rootId = match.Groups["rootid"].Value;
        var name = match.Groups["name"].Value;

        if (string.IsNullOrWhiteSpace(rootId) || string.IsNullOrWhiteSpace(name))
        {
            topicName = null!;
            return false;
        }

        topicName = new TopicName(
            capabilityRootId: CapabilityRootId.Parse(rootId),
            name: name,
            isPublic: "pub.".Equals(availability)
        );

        return true;
    }
}