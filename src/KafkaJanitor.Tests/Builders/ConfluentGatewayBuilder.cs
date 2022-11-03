using System.Net.Http;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.ConfluentCloud;
using KafkaJanitor.Tests.TestDoubles;
using Microsoft.Extensions.Logging.Abstractions;

namespace KafkaJanitor.Tests.Builders;

public class ConfluentGatewayBuilder
{
    private HttpClient? _client;
    private IConfluentCredentialsProvider _credentialsProvider;
    private IClusterRepository _clusterRepository;

    public ConfluentGatewayBuilder()
    {
        _credentialsProvider = new StubConfluentCredentialsProvider("foo", "bar");
        _clusterRepository = A.ClusterRepositoryStub
            .WithClusters(A.Cluster)
            .Build();
    }

    public ConfluentGatewayBuilder WithHttpClient(HttpClient client)
    {
        _client = client;
        return this;
    }

    public ConfluentGatewayBuilder WithConfluentCredentialsProvider(IConfluentCredentialsProvider credentialsProvider)
    {
        _credentialsProvider = credentialsProvider;
        return this;
    }

    public ConfluentGatewayBuilder WithClusterRepository(IClusterRepository clusterRepository)
    {
        _clusterRepository = clusterRepository;
        return this;
    }

    public ConfluentGateway Build()
    {
        return new ConfluentGateway(
            logger: NullLogger<ConfluentGateway>.Instance,
            client: _client!,
            credentialsProvider: _credentialsProvider,
            clusterRepository: _clusterRepository
        );
    }
}