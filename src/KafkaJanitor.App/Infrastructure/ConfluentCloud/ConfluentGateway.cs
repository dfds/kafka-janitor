using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

        var content = JsonSerializer.Deserialize<Dictionary<string, string>>(json, SerializerOptions);

        if (content is null)
        {
            _logger.LogDebug("Response content {ResponsePayload} from Confluent is not in expected format", json);
            throw new ConfluentGatewayException("Response format unknown!");
        }

        if (!content.TryGetValue("id", out var idValue))
        {
            _logger.LogDebug("Id is missing from response {ResponsePayload} from Confluent", json);
            throw new ConfluentGatewayException("Id is missing from response from Confluent");
        }

        return ServiceAccountId.Parse(idValue);
    }

    public async Task CreateACLEntry(ServiceAccountId serviceAccountId, AccessControlListEntryDescriptor entry, CancellationToken cancellationToken)
    {
        var apiKey = await _credentialsProvider.GetAccountAdminApiKey();
        
        var url = $"{ConfluentApiBaseUrl}/iam/v2/service-accounts";
        var payload = $@"{{
            ""resource_type"": ""foo bar""
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

    public Task<ClusterApiKeyDescriptor> CreateApiKey(ClusterId clusterId, ServiceAccountId serviceAccountId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
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
