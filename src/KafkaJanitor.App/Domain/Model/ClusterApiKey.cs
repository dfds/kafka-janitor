using System.Security.Cryptography;
using System.Text;

namespace KafkaJanitor.App.Domain.Model;

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