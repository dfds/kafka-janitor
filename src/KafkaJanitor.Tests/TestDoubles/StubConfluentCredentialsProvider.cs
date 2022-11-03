using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.ConfluentCloud;

namespace KafkaJanitor.Tests.TestDoubles;

public class StubConfluentCredentialsProvider : IConfluentCredentialsProvider
{
    private readonly string? _accountApiKey;
    private readonly string? _clusterApiKey;

    public StubConfluentCredentialsProvider(string? accountApiKey = null, string? clusterApiKey = null)
    {
        _accountApiKey = accountApiKey;
        _clusterApiKey = clusterApiKey;
    }

    public Task<string> GetAccountAdminApiKey() 
        => Task.FromResult(_accountApiKey!);

    public Task<string?> FindAdminApiKeyForCluster(ClusterId clusterId) 
        => Task.FromResult(_clusterApiKey);

    public static StubConfluentCredentialsProvider AsEmpty() => new StubConfluentCredentialsProvider();
}