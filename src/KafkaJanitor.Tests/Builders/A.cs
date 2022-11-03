using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.Infrastructure.ConfluentCloud;

namespace KafkaJanitor.Tests.Builders;

public static class A
{
    public static ClusterApiKeyBuilder ClusterApiKey => new();
    public static TopicBuilder Topic => new();
    public static TopicNameBuilder TopicName => new();
    public static ServiceAccountBuilder ServiceAccount => new();
    public static ServiceAccountRepositoryBuilder ServiceAccountRepository => new();
    public static TopicProvisioningProcessRepositoryBuilder TopicProvisioningProcessRepository => new();
    public static TopicProvisioningProcessBuilder TopicProvisioningProcess => new();
    public static ClusterRepositoryBuilder ClusterRepository => new();
    public static StubClusterRepositoryBuilder ClusterRepositoryStub => new();
    public static ClusterBuilder Cluster => new();
    public static ClusterId ClusterId => ClusterId.Parse("foo");
    public static ConfluentGatewayBuilder ConfluentGateway => new();
    public static JsonBasedConfluentCredentialsProviderBuilder JsonBasedConfluentCredentialsProvider => new();
    
    public static TopicPartition TopicPartition => TopicPartition.From(1);
    public static TopicRetention TopicRetention => TopicRetention.FromMilliseconds(1);
    
    public static ServiceAccountId ServiceAccountId => ServiceAccountId.Parse("foo");

    public static AccessControlListEntryDescriptorBuilder AccessControlListEntryDescriptor => new();
}