using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Application;

public class TopicProvisioningApplicationService : ITopicProvisioningApplicationService
{
    private readonly ILogger<TopicProvisioningApplicationService> _logger;
    private readonly ITopicProvisioningProcessRepository _provisioningProcessRepository;
    private readonly IConfluentGateway _confluentGateway;
    private readonly IServiceAccountRepository _serviceAccountRepository;
    private readonly IClusterRepository _clusterRepository;
    private readonly IPasswordVaultGateway _passwordVaultGateway;

    public TopicProvisioningApplicationService(ILogger<TopicProvisioningApplicationService> logger, ITopicProvisioningProcessRepository provisioningProcessRepository,
        IConfluentGateway confluentGateway, IServiceAccountRepository serviceAccountRepository, IClusterRepository clusterRepository, 
        IPasswordVaultGateway passwordVaultGateway)
    {
        _logger = logger;
        _provisioningProcessRepository = provisioningProcessRepository;
        _confluentGateway = confluentGateway;
        _serviceAccountRepository = serviceAccountRepository;
        _clusterRepository = clusterRepository;
        _passwordVaultGateway = passwordVaultGateway;
    }

    public async Task<TopicProvisionProcessId> StartProvisioningProcess(TopicName requestedTopic, ClusterId clusterId, TopicPartition partitions, TopicRetention retention)
    {
        _logger.LogDebug("Starting a topic provisioning process for topic {Topic}", requestedTopic);

        var process = TopicProvisioningProcess.Start(requestedTopic, clusterId, partitions, retention);
        await _provisioningProcessRepository.Add(process);

        _logger.LogInformation("Topic provisioning process {TopicProvisioningProcessId} has been started for topic {Topic}", process.Id, requestedTopic);

        return process.Id;
    }

    public async Task CreateServiceAccount(TopicProvisionProcessId processId)
    {
        _logger.LogDebug("Ensuring capability from topic provisioning process {TopicProvisioningProcessId} has a service account", processId);

        var process = await _provisioningProcessRepository.Get(processId);

        var serviceAccount = await _serviceAccountRepository.FindBy(process.CapabilityRootId);
        if (serviceAccount is not null)
        {
            _logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId}", serviceAccount.Id, process.CapabilityRootId, process.Id);
            process.RegisterThatCapabilityHasServiceAccount();
        }
        else
        {
            var serviceAccountId = await _confluentGateway.CreateServiceAccount(
                process.CapabilityRootId.ToString(),
                "Created using Kafka Janitor"
            );

            // TODO [jandr@2022-10-26]: handle that the service account already exists in confluent

            var newServiceAccount = ServiceAccount.DefineNew(serviceAccountId, process.CapabilityRootId);
            await _serviceAccountRepository.Add(newServiceAccount);

            _logger.LogInformation("Confluent service account {ServiceAccountId} has been defined for capability {CapabilityRootId}", 
                serviceAccountId, process.CapabilityRootId);
        }
    }

    public async Task RegisterServiceAccountForProcess(ServiceAccountId serviceAccountId)
    {
        _logger.LogDebug("Registering that service account {ServiceAccountId} has been created on all provisioning processes for a specific capability", serviceAccountId);

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        var processes = await _provisioningProcessRepository.FindAllBy(serviceAccount.CapabilityRootId);

        // NOTE [jandr@2022-10-26]: this potentially mutates multiple aggregates withing the same transaction - but they are of the same type and as a compromise will do for now
        foreach (var process in processes)
        {
            _logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId}", serviceAccount.Id, process.CapabilityRootId, process.Id);
            process.RegisterThatCapabilityHasServiceAccount();
        }
    }

    public async Task CreateMissingACLEntry(ServiceAccountId serviceAccountId)
    {
        _logger.LogDebug("Registering that service account {ServiceAccountId} has been created on all provisioning processes for a specific capability", serviceAccountId);

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        if (!serviceAccount.TryGetUnAssignedAccessControlEntry(out var entry))
        {
            // nothing!
            return;
        }

        _logger.LogDebug("Service account {ServiceAccountId} has an unassigned acl entry of {AccessControlListEntry}", serviceAccountId, entry);
        
        _logger.LogDebug("Creating acl entry in confluent");
        await _confluentGateway.CreateACLEntry(serviceAccount.Id, entry.Descriptor);

        _logger.LogDebug("Registering acl entry has been assigned");
        serviceAccount.RegisterAccessControlListEntryAsAssigned(entry.Id);
    }

    public async Task RegisterServiceAccountHasAccess(ServiceAccountId serviceAccountId)
    {
        // TODO [jandr@2022-10-26]: log

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        var processes = await _provisioningProcessRepository.FindAllBy(serviceAccount.CapabilityRootId);

        // NOTE [jandr@2022-10-26]: this potentially mutates multiple aggregates withing the same transaction - but they are of the same type and as a compromise it will do for now
        foreach (var process in processes)
        {
            _logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId} now has access!", serviceAccount.Id, process.CapabilityRootId, process.Id);
            process.RegisterThatServiceAccountHasAccess();
        }
    }

    public async Task AssignNextMissingApiKeyForServiceAccount(ServiceAccountId serviceAccountId)
    {
        _logger.LogTrace("Assigning next (if any) api key to service account {ServiceAccountId}", serviceAccountId);

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        var clusters = await _clusterRepository.GetAll();
        var nextCluster = clusters.FirstOrDefault(cluster => !serviceAccount.HasApiKeyFor(cluster.Id));

        if (nextCluster is not null)
        {
            _logger.LogDebug("Creating api key in Confluent for {ServiceAccountId} for cluster {ClusterId}", serviceAccountId, nextCluster.Id);
            var apiKey = await _confluentGateway.CreateApiKey(nextCluster.Id, serviceAccount.Id);

            _logger.LogDebug("Assigning api key {ApiKey} to service account {ServiceAccountId} for cluster {ClusterId}", apiKey.UserName, serviceAccountId, nextCluster.Id);
            serviceAccount.AssignClusterApiKey(apiKey.ClusterId, apiKey.UserName, apiKey.Password);
        }
        else
        {
            _logger.LogTrace("Service account {ServiceAccountId} has api keys for all clusters", serviceAccountId);
            var processes = await _provisioningProcessRepository.FindAllActiveBy(serviceAccount.CapabilityRootId);

            // NOTE [jandr@2022-10-28]: this potentially mutates multiple aggregates withing the same transaction - but they are of the same type and as a compromise it will do for now
            foreach (var process in processes)
            {
                _logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId} now has api keys for all clusters!", serviceAccount.Id, process.CapabilityRootId, process.Id);
                process.RegisterThatServiceAccountHasAllApiKeys();
            }
        }
    }

    public async Task StoreApiKeyInVault(ServiceAccountId serviceAccountId, ClusterApiKeyId apiKeyId)
    {
        _logger.LogTrace("Storing api key {ClusterApiKeyId} for service account {ServiceAccountId} in vault", apiKeyId, serviceAccountId);

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        if (serviceAccount.FindApiKeyBy(apiKeyId, out var apiKey))
        {
            _logger.LogDebug("Sending api key {ClusterApiKeyId} for service account {ServiceAccountId} to vault", apiKey.Id, serviceAccountId);
            await _passwordVaultGateway.Store(serviceAccount.CapabilityRootId, apiKey.ToDescriptor());
            serviceAccount.RegisterApiKeyAsStoredInVault(apiKeyId);
        }
        else
        {
            _logger.LogWarning("Service account {ServiceAccountId} does not have cluster api key {ClusterApiKeyId}", serviceAccountId, apiKeyId);
            // NOTE [jandr@2022-10-28]: consider throwing a domain exception here..!
        }
    }

    public async Task UpdateProcessWhenAllApiKeysAreStoredInVault(ServiceAccountId serviceAccountId)
    {
        _logger.LogTrace("Trying to update a provisioning process for service account {ServiceAccountId} with api keys stored in vault status", serviceAccountId);

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        var clusters = await _clusterRepository.GetAll();

        var hasAllKeysStoredInVault = true;

        // NOTE [jandr@2022-10-28]: this logic would also make sense in a domain service
        foreach (var cluster in clusters)
        {
            var apiKeys = serviceAccount.FindApiKeysBy(cluster.Id);
            if (!apiKeys.Any())
            {
                _logger.LogTrace("Service account {ServiceAccountId} does not have any api keys for cluster {ClusterId} yet", serviceAccountId, cluster.Id);
                hasAllKeysStoredInVault = false;
                break;
            }

            if (apiKeys.Any(x => !x.IsStoredInVault))
            {
                _logger.LogTrace("Service account {ServiceAccountId} has an api key for cluster {ClusterId} that is not yet stored in vault", serviceAccountId, cluster.Id);
                hasAllKeysStoredInVault = false; 
            }
        }

        if (hasAllKeysStoredInVault)
        {
            _logger.LogDebug("Service account {ServiceAccountId} has all of its api keys stored in vault", serviceAccountId);
            var processes = await _provisioningProcessRepository.FindAllActiveBy(serviceAccount.CapabilityRootId);

            foreach (var process in processes)
            {
                _logger.LogDebug("Registering that service account {ServiceAccountId} has all api keys stored in vault on provisioning process {TopicProvisioningProcessId}", serviceAccountId, process.Id);
                process.RegisterThatAllApiKeysAreStoredInVault();
            }
        }
    }

    public async Task CreateTopicFrom(TopicProvisionProcessId processId)
    {
        _logger.LogTrace("Trying to create topic from topic provisioning process {TopicProvisioningProcessId}", processId);

        var process = await _provisioningProcessRepository.Get(processId);

        // TODO [jandr@2022-10-24]: handle cases where the topic already exists
        //_confluentGateway.Exists()

        await _confluentGateway.CreateTopic(
            clusterId: process.ClusterId, 
            topic: process.RequestedTopic,
            partition: process.Partitions,
            retention: process.Retention
        );

        process.RegisterTopicAsProvisioned();

        _logger.LogInformation("Topic {Topic} has been provisioned for capability {CapabilityRootId} in cluster {ClusterId}", 
            process.RequestedTopic, process.CapabilityRootId, process.ClusterId);
    }
}