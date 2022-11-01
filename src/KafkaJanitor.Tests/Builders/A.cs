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
    public static ClusterBuilder Cluster => new();
}