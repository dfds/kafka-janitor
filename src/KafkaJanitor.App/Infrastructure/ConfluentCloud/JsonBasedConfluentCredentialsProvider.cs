using System.Text.Json;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Infrastructure.ConfluentCloud;

public class JsonBasedConfluentCredentialsProvider : IConfluentCredentialsProvider
{
    private readonly ITextContentReader _reader;

    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonBasedConfluentCredentialsProvider(ITextContentReader reader)
    {
        _reader = reader;
    }

/*
Supported structure:
-----------------------------------
{
    "account": "lala",
    "clusters": [
        {
            "id": "foo",
            "apiKey": "foo-key"
        },
        {
            "id": "bar",
            "apiKey": "bar-key"
        }
    ]
}
*/

    private async Task<Content?> ReadContent()
    {
        var lines = await _reader.ReadAllLines();
        var json = string.Join(" ", lines);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<Content>(json, SerializerOptions);
    }

    public async Task<string> GetAccountAdminApiKey()
    {
        var content = await ReadContent();
        if (content is null)
        {
            return "";
        }

        return content.Account;
    }

    public async Task<string?> FindAdminApiKeyForCluster(ClusterId clusterId)
    {
        var content = await ReadContent();
        if (content?.Clusters is null)
        {
            return null;
        }

        var result = content
            .Clusters
            .FirstOrDefault(x => clusterId.ToString().Equals(x.Id, StringComparison.InvariantCultureIgnoreCase));

        return result?.ApiKey;
    }

    public record Content(string Account, ClusterCredential[] Clusters);
    public record ClusterCredential(string Id, string ApiKey);
}