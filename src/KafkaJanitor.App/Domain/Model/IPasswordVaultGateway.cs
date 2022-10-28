namespace KafkaJanitor.App.Domain.Model;

public interface IPasswordVaultGateway
{
    Task Store(CapabilityRootId capabilityRootId, ClusterApiKeyDescriptor apiKeyDescriptor);
}