namespace KafkaJanitor.App.Domain.Model;

public interface IServiceAccountRepository
{
    Task<ServiceAccount?> FindBy(CapabilityRootId rootId);
    Task Add(ServiceAccount serviceAccount);
    Task<ServiceAccount> Get(ServiceAccountId serviceAccountId);
}