namespace KafkaJanitor.App.Domain.Model;

public interface IClusterAccessDefinitionRepository
{
    Task Add(ClusterAccessDefinition definition);
    Task<ClusterAccessDefinition?> FindBy(ClusterId clusterId, ServiceAccountId serviceAccountId);
    Task<ClusterAccessDefinition> Get(ClusterAccessDefinitionId id);
}