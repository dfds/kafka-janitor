using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Infrastructure.ConfluentCloud;

public class ConfluentGateway : IConfluentGateway
{
    private static readonly string ConfluentApiBaseUrl = "https://api.confluent.cloud";
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ILogger<ConfluentGateway> _logger;
    private readonly HttpClient _client;
    private readonly IConfluentCredentialsProvider _credentialsProvider;
    private readonly IClusterRepository _clusterRepository;

    public ConfluentGateway(ILogger<ConfluentGateway> logger, HttpClient client, IConfluentCredentialsProvider credentialsProvider, 
        IClusterRepository clusterRepository)
    {
        _logger = logger;
        _client = client;
        _credentialsProvider = credentialsProvider;
        _clusterRepository = clusterRepository;
    }

    [Obsolete]
    public Task<IEnumerable<Topic>> GetAllBy(ClusterId clusterId)
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public Task<bool> Exists(string topicName)
    {
        throw new NotImplementedException();
    }

    public async Task CreateTopic(ClusterId clusterId, TopicName topic, TopicPartition partition, TopicRetention retention, CancellationToken cancellationToken)
    {
        var apiKey = await _credentialsProvider.FindAdminApiKeyForCluster(clusterId);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ConfluentGatewayException($"An api key for cluster \"{clusterId}\" could not be found.");
        }

        var retentionConfig = retention == TopicRetention.Infinite
            ? "-1"
            : retention.ToMilliseconds().ToString();

        var payload = $@"{{
            ""topic_name"": ""{topic}"",
            ""partition_count"": {partition.ToString()},
            ""replication_factor"": 3,
            ""configs"": [
                {{
                    ""name"": ""retention.ms"",
                    ""value"": ""{retentionConfig}""
                }}
            ]
            }}";

        var cluster = await _clusterRepository.Get(clusterId);
        var url = $"{cluster.AdminApiEndpoint.TrimEnd('/')}/kafka/v3/clusters/{clusterId}/topics";

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request, cancellationToken);
        _logger.LogTrace("Received response status code {StatusCode} from {ConfluentUrl}", response.StatusCode, url);

        if (!response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Response from Confluent was {StatusCode} with payload {ResponsePayload}", response.StatusCode, json);
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task<ServiceAccountId> CreateServiceAccount(string name, string description, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or whitespace.", nameof(description));
        }

        var url = $"{ConfluentApiBaseUrl}/iam/v2/service-accounts";

        var payload = $@"{{
            ""display_name"": ""{name}"",
            ""description"": ""{description}""
        }}";

        var apiKey = await _credentialsProvider.GetAccountAdminApiKey();

        _logger.LogTrace("Sending request to {ConfluentUrl} for creating a new service account with payload {Payload}", url, payload);

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request, cancellationToken);
        _logger.LogTrace("Received response status code {StatusCode} from {ConfluentUrl}", response.StatusCode, url);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            _logger.LogWarning("Service account with name {ServiceAccountName} already exists", name);
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebug("Response from Confluent was {StatusCode} with payload {ResponsePayload}", response.StatusCode, json);
        }

        response.EnsureSuccessStatusCode();

        var content = JsonSerializer.Deserialize<ServiceAccountResponse>(json, SerializerOptions);

        if (content is null)
        {
            _logger.LogDebug("Response content {ResponsePayload} from Confluent is not in expected format", json);
            throw new ConfluentGatewayException("Response format unknown!");
        }

        if (!ServiceAccountId.TryParse(content.Id, out var serviceAccountId))
        {
            _logger.LogDebug("Valid service account id is missing in response {ResponsePayload} from Confluent", json);
            throw new ConfluentGatewayException("Valid service account id is missing in response from Confluent");
        }

        return serviceAccountId;
    }

    public async Task CreateACLEntry(ClusterId clusterId, ServiceAccountId serviceAccountId, AccessControlListEntryDescriptor entry, CancellationToken cancellationToken)
    {
        var apiKey = await _credentialsProvider.FindAdminApiKeyForCluster(clusterId);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ConfluentGatewayException($"An api key for cluster \"{clusterId}\" could not be found.");
        }

        var cluster = await _clusterRepository.Get(clusterId);
        var url = $"{cluster.AdminApiEndpoint.TrimEnd('/')}/kafka/v3/clusters/{clusterId}/acls";

        var payload = $@"{{
            ""resource_type"": ""{ToString(entry.ResourceType)}"",
            ""resource_name"": ""{entry.ResourceName}"",
            ""pattern_type"": ""{ToString(entry.PatternType)}"",
            ""principal"": ""User:{serviceAccountId}"",
            ""host"": ""*"",
            ""operation"": ""{ToString(entry.OperationType)}"",
            ""permission"": ""{ToString(entry.PermissionType)}""
        }}";

        _logger.LogTrace("Sending request to {ConfluentUrl} for creating a new acl entry with payload {Payload}", url, payload);

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request, cancellationToken);
        _logger.LogTrace("Received response status code {StatusCode} from {ConfluentUrl}", response.StatusCode, url);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Response from Confluent was {StatusCode} with payload {ResponsePayload}", response.StatusCode, content);
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task<ClusterApiKeyDescriptor> CreateApiKey(ClusterId clusterId, ServiceAccountId serviceAccountId, CancellationToken cancellationToken)
    {
        var url = $"{ConfluentApiBaseUrl}/iam/v2/api-keys"; 

        var payload = $@"{{
            ""spec"": {{
                ""display_name"": """",
                ""description"": ""Created by Kafka Janitor"",
                ""owner"": {{
                    ""id"": ""{serviceAccountId}""
                }},
                ""resource"": {{
                    ""id"": ""{clusterId}""
                }}
            }}
        }}";

        var apiKey = await _credentialsProvider.GetAccountAdminApiKey();

        _logger.LogTrace("Sending request to {ConfluentUrl} for creating an api key in cluster {ClusterId} for service account {ServiceAccountId} with payload {Payload}", 
            url, clusterId, serviceAccountId, payload);

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request, cancellationToken);
        _logger.LogTrace("Received response status code {StatusCode} from {ConfluentUrl}", response.StatusCode, url);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebug("Response from Confluent was {StatusCode} with payload {ResponsePayload}", response.StatusCode, json);
        }

        response.EnsureSuccessStatusCode();

        var content = JsonSerializer.Deserialize<ApiKeyResponse>(json, SerializerOptions);

        if (content is null)
        {
            _logger.LogDebug("Response content {ResponsePayload} from Confluent is not in expected format", json);
            throw new ConfluentGatewayException("Response format unknown!");
        }

        if (string.IsNullOrEmpty(content.Id))
        {
            _logger.LogDebug("Id is missing from response {ResponsePayload} from Confluent", json);
            throw new ConfluentGatewayException("Id is missing from response from Confluent");
        }

        if (content.Spec is null)
        {
            _logger.LogDebug("Spec section is missing from response {ResponsePayload} from Confluent", json);
            throw new ConfluentGatewayException("Spec section is missing from response from Confluent");
        }

        if (string.IsNullOrEmpty(content.Spec.Secret))
        {
            _logger.LogDebug("Spec.Secret is missing from response {ResponsePayload} from Confluent", json);
            throw new ConfluentGatewayException("Spec.Secret is missing from response from Confluent");
        }

        if (content.Spec.Resource is null)
        {
            _logger.LogDebug("Spec.Resource section is missing from response {ResponsePayload} from Confluent", json);
            throw new ConfluentGatewayException("Spec.Resource section is missing from response from Confluent");
        }

        if (!ClusterId.TryParse(content.Spec.Resource.Id, out var actualClusterId))
        {
            _logger.LogDebug("Spec.Resource.Id is missing from response {ResponsePayload} from Confluent", json);
            throw new ConfluentGatewayException("Spec.Resource.Id is missing from response from Confluent");
        }

        return new ClusterApiKeyDescriptor(actualClusterId, content.Id, content.Spec.Secret);
    }

    #region response payloads

    private class ServiceAccountResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    private class ApiKeyResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("spec")]
        public SpecSection? Spec { get; set; }

        public class SpecSection
        {
            [JsonPropertyName("secret")]
            public string? Secret { get; set; }

            [JsonPropertyName("resource")]
            public ResourceSection? Resource { get; set; }

            public class ResourceSection
            {
                [JsonPropertyName("id")]
                public string? Id { get; set; }
            }
        }
    }

    #endregion

    #region to string helpers

    private static string ToString(ACLEntryPermissionType type) => type switch
    {
        ACLEntryPermissionType.Deny => "DENY",
        ACLEntryPermissionType.Allow => "ALLOW",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    private static string ToString(ACLEntryOperationType type) => type switch
    {
        ACLEntryOperationType.Create => "CREATE",
        ACLEntryOperationType.Read => "READ",
        ACLEntryOperationType.Write => "WRITE",
        ACLEntryOperationType.Describe => "DESCRIBE",
        ACLEntryOperationType.DescribeConfigs => "DESCRIBE_CONFIGS",
        ACLEntryOperationType.Alter => "ALTER",
        ACLEntryOperationType.AlterConfigs => "ALTER_CONFIGS",
        ACLEntryOperationType.ClusterAction => "CLUSTER_ACTION",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    private static string ToString(ACLEntryPatternType type) => type switch
    {
        ACLEntryPatternType.Literal => "LITERAL",
        ACLEntryPatternType.Prefix => "PREFIXED",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    private static string ToString(ACLEntryResourceType type) => type switch
    {
        ACLEntryResourceType.Topic => "TOPIC",
        ACLEntryResourceType.Group => "GROUP",
        ACLEntryResourceType.Cluster => "CLUSTER",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    #endregion
}

public interface IConfluentCredentialsProvider
{
    Task<string> GetAccountAdminApiKey();
    Task<string?> FindAdminApiKeyForCluster(ClusterId clusterId);
}

public class ConfluentGatewayException : Exception
{
    public ConfluentGatewayException(string message) : base(message)
    {
        
    }
}
