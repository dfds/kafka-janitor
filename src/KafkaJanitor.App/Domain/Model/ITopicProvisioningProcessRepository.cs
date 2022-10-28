namespace KafkaJanitor.App.Domain.Model;

public interface ITopicProvisioningProcessRepository
{
    Task Add(TopicProvisioningProcess process);
    Task<TopicProvisioningProcess> Get(TopicProvisionProcessId id);
    Task<IEnumerable<TopicProvisioningProcess>> FindAllBy(CapabilityRootId capabilityRootId);
    Task<IEnumerable<TopicProvisioningProcess>> FindAllActiveBy(CapabilityRootId capabilityRootId);
}