using KafkaJanitor.App.Domain.Events;

namespace KafkaJanitor.App.Domain.Model;

public class ServiceAccount : AggregateRoot<ServiceAccountId>
{
    private readonly List<ClusterApiKey> _clusterApiKeys = null!;

    private ServiceAccount() { }

    public ServiceAccount(ServiceAccountId id, CapabilityRootId capabilityRootId, IEnumerable<ClusterApiKey> clusterApiKeys) : base(id)
    {
        if (capabilityRootId == null)
        {
            throw new ArgumentNullException(nameof(capabilityRootId), "Capability root id value cannot be null.");
        }

        CapabilityRootId = capabilityRootId;
        _clusterApiKeys = new List<ClusterApiKey>(clusterApiKeys);
    }

    public CapabilityRootId CapabilityRootId { get; private set; } = null!;

    public IEnumerable<ClusterApiKey> ClusterApiKeys => _clusterApiKeys;

    public void AssignClusterApiKey(ClusterId clusterId, string userName, string password)
    {
        // TODO [jandr@2022-10-27]: null/empty checks on parameter values or convert back to injecting the descriptor instead

        var apiKey = ClusterApiKey.Create(userName, password, clusterId);
        _clusterApiKeys.Add(apiKey);

        Raise(new ClusterApiKeyHasBeenAssigned
        {
            ServiceAccountId = Id.ToString(),
            ClusterApiKeyId = apiKey.Id.ToString(),
        });
    }

    public bool FindApiKeyBy(ClusterApiKeyId id, out ClusterApiKey apiKey)
    {
        var key = _clusterApiKeys.SingleOrDefault(x => x.Id == id);
        if (key is null)
        {
            apiKey = null!;
            return false;
        }

        apiKey = key;
        return true;
    }

    public bool FindApiKeyBy(ClusterId clusterId, out ClusterApiKey apiKey)
    {
        var key = _clusterApiKeys.SingleOrDefault(x => x.ClusterId == clusterId);
        if (key is null)
        {
            apiKey = null!;
            return false;
        }

        apiKey = key;
        return true;
    }

    public void RegisterApiKeyAsStoredInVault(ClusterApiKeyId apiKeyId)
    {
        if (!FindApiKeyBy(apiKeyId, out var apiKey))
        {
            throw new Exception($"Unknown api key for {apiKeyId}");
        }

        apiKey.MarkAsStoredInVault();

        Raise(new ApiKeyHasBeenStoredInVault
        {
            ServiceAccountId = Id.ToString(),
            ClusterApiKeyId = apiKey.Id.ToString()
        });
    }

    public static ServiceAccount DefineNew(ServiceAccountId id, CapabilityRootId capabilityRootId)
    {
        var serviceAccount = new ServiceAccount(
            id: id,
            capabilityRootId: capabilityRootId,
            clusterApiKeys: Enumerable.Empty<ClusterApiKey>()
        );

        serviceAccount.Raise(new ServiceAccountHasBeenDefined
        {
            ServiceAccountId = serviceAccount.Id.ToString()
        });

        return serviceAccount;
    }
}