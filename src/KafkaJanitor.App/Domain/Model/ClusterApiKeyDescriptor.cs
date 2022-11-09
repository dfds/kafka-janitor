namespace KafkaJanitor.App.Domain.Model;

public class ClusterApiKeyDescriptor : ValueObject
{
    public ClusterApiKeyDescriptor(ClusterId clusterId, string userName, string password)
    {
        if (clusterId is null)
        {
            throw new ArgumentNullException(nameof(clusterId));
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