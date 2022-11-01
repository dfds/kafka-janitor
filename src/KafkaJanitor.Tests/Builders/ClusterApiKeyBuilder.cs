using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class ClusterApiKeyBuilder
{
    private ClusterApiKeyId _id;
    private string _userName;
    private string _password;
    private string _passwoordChecksum;
    private ClusterId _clusterId;
    private bool _isStoredInVault;

    public ClusterApiKeyBuilder()
    {
        _id = ClusterApiKeyId.New();
        _userName = "foo";
        _password = "bar";
        _passwoordChecksum = "baz";
        _clusterId = ClusterId.None;
        _isStoredInVault = true;
    }

    public ClusterApiKey Build()
    {
        return new ClusterApiKey(_id, _userName, _password, _passwoordChecksum, _clusterId, _isStoredInVault);
    }

    public static implicit operator ClusterApiKey(ClusterApiKeyBuilder builder) 
        => builder.Build();
}