using System.Security.Cryptography;
using System.Text;
using KafkaJanitor.App.Domain.Events;
using static KafkaJanitor.App.Domain.Model.ACLEntryOperationType;
using static KafkaJanitor.App.Domain.Model.ACLEntryPermissionType;

namespace KafkaJanitor.App.Domain.Model;

public class ServiceAccount : AggregateRoot<ServiceAccountId>
{
    private readonly IList<ClusterApiKey> _clusterApiKeys = new List<ClusterApiKey>();

    public ServiceAccount(ServiceAccountId id, CapabilityRootId capabilityRootId, IEnumerable<AccessControlListEntry> accessControlList) : base(id)
    {
        if (capabilityRootId == null)
        {
            throw new ArgumentNullException(nameof(capabilityRootId), "Capability root id value cannot be null.");
        }

        CapabilityRootId = capabilityRootId;
        AccessControlList = accessControlList;
    }

    public CapabilityRootId CapabilityRootId { get; private set; }
    public IEnumerable<AccessControlListEntry> AccessControlList { get; private set; }
    public IEnumerable<ClusterApiKey> ClusterApiKeys => _clusterApiKeys;

    public bool HasApiKeyFor(ClusterId cluster) 
        => _clusterApiKeys.Any(x => x.ClusterId == cluster);

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

    public IEnumerable<ClusterApiKey> FindApiKeysBy(ClusterId clusterId) 
        => _clusterApiKeys.Where(x => x.ClusterId == clusterId);

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

    public bool TryGetUnAssignedAccessControlEntry(out AccessControlListEntry entry)
    {
        var unApplied = AccessControlList.FirstOrDefault(x => !x.IsApplied);

        entry = unApplied!;
        return unApplied is not null;
    }

    public void RegisterAccessControlListEntryAsAssigned(AccessControlListEntryId entryId)
    {
        var entry = AccessControlList.SingleOrDefault(x => x.Id == entryId);
        if (entry is null)
        {
            throw new Exception($"Access control list entry \"{entryId}\" does not belong to service account \"{Id}\".");
        }

        if (entry.IsApplied)
        {
            return;
        }

        entry.RegisterAsAssigned();

        Raise(new ACLEntryHasBeenAssigned
        {
            ServiceAccountId = Id.ToString(),
            AccessControlListEntryId = entry.Id.ToString()
        });

        if (AccessControlList.All(x => x.IsApplied))
        {
            Raise(new AllACLEntriesHasBeenAssigned
            {
                ServiceAccountId = Id.ToString()
            });
        }
    }

    public static ServiceAccount DefineNew(ServiceAccountId id, CapabilityRootId capabilityRootId)
    {
        var acl = CreateDefaultAccessControlList(capabilityRootId);

        var serviceAccount = new ServiceAccount(
            id: id,
            capabilityRootId: capabilityRootId,
            accessControlList: acl
        );

        serviceAccount.Raise(new ServiceAccountHasBeenDefined
        {
            ServiceAccountId = serviceAccount.Id.ToString()
        });

        return serviceAccount;
    }

    private static IEnumerable<AccessControlListEntry> CreateDefaultAccessControlList(CapabilityRootId capabilityRootId)
    {
        // deny create operations on all resource types
        yield return AccessControlListEntry.CreateForTopicPrefix("'*'", Create, Deny);

        // for all private topics
        yield return AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Read, Allow);
        yield return AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Write, Allow);
        yield return AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Create, Allow);
        yield return AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), Describe, Allow);
        yield return AccessControlListEntry.CreateForTopicPrefix(capabilityRootId.ToString(), DescribeConfigs, Allow);

        // for all public topics
        yield return AccessControlListEntry.CreateForTopicPrefix("pub.", Read, Allow);

        // for own public topics
        yield return AccessControlListEntry.CreateForTopicPrefix($"pub.{capabilityRootId}.", Write, Allow);
        yield return AccessControlListEntry.CreateForTopicPrefix($"pub.{capabilityRootId}.", Create, Allow);

        // for all connect groups
        yield return AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", Read, Allow);
        yield return AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", Write, Allow);
        yield return AccessControlListEntry.CreateForGroupPrefix($"connect-{capabilityRootId}", Create, Allow);

        // for all capability groups
        yield return AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), Read, Allow);
        yield return AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), Write, Allow);
        yield return AccessControlListEntry.CreateForGroupPrefix(capabilityRootId.ToString(), Create, Allow);

        // for cluster
        yield return AccessControlListEntry.Create(
            resourceType: ACLEntryResourceType.Cluster,
            resourceName: "kafka-cluster",
            patternType: ACLEntryPatternType.Literal,
            operationType: Alter,
            permissionType: Deny
        );

        yield return AccessControlListEntry.Create(
            resourceType: ACLEntryResourceType.Cluster,
            resourceName: "kafka-cluster",
            patternType: ACLEntryPatternType.Literal,
            operationType: AlterConfigs, 
            permissionType: Deny
        );

        yield return AccessControlListEntry.Create(
            resourceType: ACLEntryResourceType.Cluster,
            resourceName: "kafka-cluster",
            patternType: ACLEntryPatternType.Literal,
            operationType: ClusterAction,
            permissionType: Deny
        );
    }
}

public class ApiKeyHasBeenStoredInVault : IDomainEvent
{
    public string? ServiceAccountId { get; set; }
    public string? ClusterApiKeyId { get; set; }
}

public class ClusterApiKey : Entity<ClusterApiKeyId>
{
    public ClusterApiKey(ClusterApiKeyId id, string userName, string password, string passwordChecksum, ClusterId clusterId, bool isStoredInVault) : base(id)
    {
        UserName = userName;
        Password = password;
        PasswordChecksum = passwordChecksum;
        ClusterId = clusterId;
        IsStoredInVault = isStoredInVault;
    }

    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string PasswordChecksum { get; private set; }
    public ClusterId ClusterId { get; private set; }
    public bool IsStoredInVault { get; private set; }

    public void Anonymize() => Password = "***";

    public void MarkAsStoredInVault() => IsStoredInVault = true;

    public ClusterApiKeyDescriptor ToDescriptor() 
        => new ClusterApiKeyDescriptor(ClusterId, UserName, Password);

    public static ClusterApiKey Create(string userName, string password, ClusterId clusterId)
    {
        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(password));
        var checksum = BitConverter
            .ToString(hash)
            .Replace("-", "");

        return new ClusterApiKey(
            id: ClusterApiKeyId.New(),
            userName: userName,
            password: password,
            passwordChecksum: checksum,
            clusterId: clusterId,
            isStoredInVault: false
        );
    }
}

public class ClusterApiKeyId : ValueObject
{
    private readonly Guid _value;

    private ClusterApiKeyId(Guid value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString() 
        => _value.ToString();

    public static ClusterApiKeyId Parse(string? text)
    {
        if (TryParse(text, out var id))
        {
            return id;
        }

        throw new FormatException($"Value \"{text}\" is not a valid api key id");
    }

    public static bool TryParse(string? text, out ClusterApiKeyId id)
    {
        if (Guid.TryParse(text, out var value))
        {
            id = new ClusterApiKeyId(value);
            return true;
        }

        id = null!;
        return false;
    }

    public static ClusterApiKeyId New() => new ClusterApiKeyId(Guid.NewGuid());

    public static implicit operator Guid(ClusterApiKeyId id)
        => id._value;

    public static implicit operator ClusterApiKeyId(Guid id)
        => new ClusterApiKeyId(id);
}

public class ClusterApiKeyHasBeenAssigned : IDomainEvent
{
    public string? ServiceAccountId { get; set; }
    public string? ClusterApiKeyId { get; set; }
}

public class ClusterApiKeyDescriptor : ValueObject
{
    public ClusterApiKeyDescriptor(ClusterId clusterId, string userName, string password)
    {
        if (ClusterId is null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
        }

        ClusterId = clusterId;
        UserName = userName;
        Password = password;
    }

    public string UserName { get; }
    public string Password { get; }
    public ClusterId ClusterId { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ClusterId;
        yield return UserName;
        yield return Password;
    }

    public override string ToString() => $"{ClusterId}:{UserName}";
}

