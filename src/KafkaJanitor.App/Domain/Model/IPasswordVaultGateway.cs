namespace KafkaJanitor.App.Domain.Model;

public interface IPasswordVaultGateway
{
    Task Store(CapabilityRootId capabilityRootId, ClusterApiKeyDescriptor apiKeyDescriptor);
}

public class PasswordVaultGateway : IPasswordVaultGateway
{
    public Task Store(CapabilityRootId capabilityRootId, ClusterApiKeyDescriptor apiKeyDescriptor)
    {
        throw new NotImplementedException();
    }
}