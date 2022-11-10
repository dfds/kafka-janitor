using KafkaJanitor.App.Domain.DomainServices;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Application;

public class TopicProvisioningApplicationService : ITopicProvisioningApplicationService
{
    private readonly ILogger<TopicProvisioningApplicationService> _logger;
    private readonly ITopicProvisioningProcessRepository _provisioningProcessRepository;
    private readonly IConfluentGateway _confluentGateway;
    private readonly IServiceAccountRepository _serviceAccountRepository;
    private readonly IPasswordVaultGateway _passwordVaultGateway;
    private readonly IClusterAccessDefinitionRepository _clusterAccessDefinitionRepository;
    private readonly ClusterAccessDomainService _clusterAccessDomainService;

    public TopicProvisioningApplicationService(ILogger<TopicProvisioningApplicationService> logger, ITopicProvisioningProcessRepository provisioningProcessRepository,
        IConfluentGateway confluentGateway, IServiceAccountRepository serviceAccountRepository, IPasswordVaultGateway passwordVaultGateway, 
        IClusterAccessDefinitionRepository clusterAccessDefinitionRepository, ClusterAccessDomainService clusterAccessDomainService)
    {
        _logger = logger;
        _provisioningProcessRepository = provisioningProcessRepository;
        _confluentGateway = confluentGateway;
        _serviceAccountRepository = serviceAccountRepository;
        _passwordVaultGateway = passwordVaultGateway;
        _clusterAccessDefinitionRepository = clusterAccessDefinitionRepository;
        _clusterAccessDomainService = clusterAccessDomainService;
    }

    [Transactional, Outboxed]
    public async Task<TopicProvisionProcessId> StartProvisioningProcess(TopicName requestedTopic, ClusterId clusterId, TopicPartition partitions, TopicRetention retention)
    {
        _logger.LogDebug("Starting a topic provisioning process for topic {Topic}", requestedTopic);

        var process = TopicProvisioningProcess.Start(requestedTopic, clusterId, partitions, retention);
        await _provisioningProcessRepository.Add(process);

        _logger.LogInformation("Topic provisioning process {TopicProvisioningProcessId} has been started for topic {Topic}", process.Id, requestedTopic);

        return process.Id;
    }

    [Transactional, Outboxed]
    public async Task EnsureCapabilityHasServiceAccount(TopicProvisionProcessId processId)
    {
        _logger.LogTrace("Ensuring capability from topic provisioning process {TopicProvisioningProcessId} has a service account", processId);

        var process = await _provisioningProcessRepository.Get(processId);

        var serviceAccount = await _serviceAccountRepository.FindBy(process.CapabilityRootId);
        if (serviceAccount is not null)
        {
            _logger.LogDebug("Capability {CapabilityRootId} already has service account {ServiceAccountId} defined", process.CapabilityRootId, serviceAccount.Id);
            //_logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId}", serviceAccount.Id, process.CapabilityRootId, process.Id);
            process.RegisterThatCapabilityHasServiceAccount();
        }
        else
        {
            _logger.LogDebug("Capability {CapabilityRootId} needs a service account - creating it in Confluent", process.CapabilityRootId);

            var serviceAccountId = await _confluentGateway.CreateServiceAccount(
                name: process.CapabilityRootId.ToString(),
                description: "Created using Kafka Janitor",
                cancellationToken: CancellationToken.None
            );

            // TODO [jandr@2022-10-26]: handle that the service account already exists in confluent

            var newServiceAccount = ServiceAccount.DefineNew(serviceAccountId, process.CapabilityRootId);
            await _serviceAccountRepository.Add(newServiceAccount);

            _logger.LogInformation("Confluent service account {ServiceAccountId} has been defined for capability {CapabilityRootId}", 
                serviceAccountId, process.CapabilityRootId);
        }
    }

    [Transactional, Outboxed]
    public async Task RegisterNewServiceAccountIsDefined(ServiceAccountId serviceAccountId)
    {
        _logger.LogTrace("Registering that new service account {ServiceAccountId} has been created on all provisioning processes for a specific capability", serviceAccountId);

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        var processes = await _provisioningProcessRepository.FindAllActiveBy(serviceAccount.CapabilityRootId);

        // NOTE [jandr@2022-10-26]: this potentially mutates multiple aggregates withing the same transaction - but they are of the same type and as a compromise will do for now
        foreach (var process in processes)
        {
            _logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId}", serviceAccount.Id, process.CapabilityRootId, process.Id);
            process.RegisterThatCapabilityHasServiceAccount();
        }
    }

    [Transactional, Outboxed]
    public async Task ApplyNextMissingACLEntry(ClusterAccessDefinitionId clusterAccessDefinitionId)
    {
        _logger.LogTrace("Applying next missing ACL entry for cluster access definition {ClusterAccessDefinitionId}", clusterAccessDefinitionId);

        var accessDefinition = await _clusterAccessDefinitionRepository.Get(clusterAccessDefinitionId);

        var unAppliedAclEntry = accessDefinition.FindNextUnAppliedAccessControlListEntry();
        if (unAppliedAclEntry == null)
        {
            _logger.LogTrace("Cluster access definition {ClusterAccessDefinitionId} does not have any unapplied acl entries", accessDefinition.Id);
            return; // nothing
        }

        _logger.LogDebug("Service account {ServiceAccountId} has an unassigned acl entry of {AccessControlListEntryId} from cluster access definition {ClusterAccessDefinitionId}", 
            accessDefinition.ServiceAccountId, unAppliedAclEntry.Id, clusterAccessDefinitionId);
        
        _logger.LogDebug("Creating acl entry {AccessControlListEntryId} in confluent", unAppliedAclEntry.Id);
        await _confluentGateway.CreateACLEntry(accessDefinition.ClusterId, accessDefinition.ServiceAccountId, unAppliedAclEntry.Descriptor, CancellationToken.None);

        _logger.LogDebug("Registering acl entry {AccessControlListEntryId} has been applied", unAppliedAclEntry.Id);
        accessDefinition.RegisterAccessControlListEntryAsApplied(unAppliedAclEntry.Id);
    }

    [Transactional, Outboxed]
    public async Task RegisterServiceAccountHasAccess(ClusterAccessDefinitionId clusterAccessDefinitionId)
    {
        _logger.LogTrace("Registering that cluster access definition {ClusterAccessDefinitionId} has completed and service account now has access", clusterAccessDefinitionId);

        var accessDefinition = await _clusterAccessDefinitionRepository.Get(clusterAccessDefinitionId);
        var serviceAccount = await _serviceAccountRepository.Get(accessDefinition.ServiceAccountId);
        var processes = await _provisioningProcessRepository.FindAllActiveBy(serviceAccount.CapabilityRootId);

        // NOTE [jandr@2022-10-26]: this potentially mutates multiple aggregates withing the same transaction - but they are of the same type and as a compromise it will do for now
        foreach (var process in processes)
        {
            _logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId} now has access to cluster {ClusterId}!", serviceAccount.Id, process.CapabilityRootId, process.Id, accessDefinition.ClusterId);
            process.RegisterThatServiceAccountHasAccessToCluster();
        }
    }

    [Transactional, Outboxed]
    public async Task EnsureServiceAccountHasApiKey(TopicProvisionProcessId processId)
    {
        _logger.LogTrace("Ensuring service account for capability on topic provisioning process {TopicProvisioningProcessId} has api key for cluster", processId);

        var process = await _provisioningProcessRepository.Get(processId);

        var serviceAccount = await _serviceAccountRepository.FindBy(process.CapabilityRootId);
        if (serviceAccount is null)
        {
            throw new Exception("nooooo service account is null!"); // TODO [jandr@2022-11-08]: change/rethink exception
        }

        if (serviceAccount.FindApiKeyBy(process.ClusterId, out var apiKey))
        {
            if (apiKey.IsStoredInVault)
            {
                _logger.LogDebug("Registering service account {ServiceAccountId} for capability {CapabilityRootId} on process {TopicProvisioningProcessId} now has api keys for all clusters!",
                    serviceAccount.Id, process.CapabilityRootId, process.Id);
                process.RegisterThatApiKeyIsStoredInVault();
            }
            else
            {
                _logger.LogDebug("Cluster api key {ClusterApiKeyId} for service account {ServiceAccountId} should be in the process of being stored in a vault...", apiKey.Id, serviceAccount.Id);
            }
        }
        else
        {
            _logger.LogDebug("Creating api key in Confluent for {ServiceAccountId} in cluster {ClusterId}", processId, process.ClusterId);
            var newApiKey = await _confluentGateway.CreateApiKey(process.ClusterId, serviceAccount.Id, CancellationToken.None);

            _logger.LogDebug("Assigning api key {ApiKey} to service account {ServiceAccountId} for cluster {ClusterId}", newApiKey.UserName, processId, process.ClusterId);
            serviceAccount.AssignClusterApiKey(newApiKey.ClusterId, newApiKey.UserName, newApiKey.Password);
        }
    }

    [Transactional, Outboxed]
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

            throw new Exception("nooooo api key does not belong to service account"); // TODO [jandr@2022-11-08]: rename/rethink exception
        }
    }

    [Transactional, Outboxed]
    public async Task UpdateProcessWhenApiKeyIsStoredInVault(ServiceAccountId serviceAccountId)
    {
        _logger.LogTrace("Trying to update a provisioning process for service account {ServiceAccountId} with api key stored in vault status", serviceAccountId);

        var serviceAccount = await _serviceAccountRepository.Get(serviceAccountId);
        var processes = await _provisioningProcessRepository.FindAllActiveBy(serviceAccount.CapabilityRootId);

        foreach (var process in processes)
        {
            _logger.LogDebug("Registering that service account {ServiceAccountId} has api key for cluster {ClusterId} stored in vault on provisioning process {TopicProvisioningProcessId}",
                serviceAccountId, process.ClusterId, process.Id);

            process.RegisterThatApiKeyIsStoredInVault();
        }
    }

    [Transactional, Outboxed]
    public async Task ProvisionTopicFrom(TopicProvisionProcessId processId)
    {
        _logger.LogTrace("Trying to create topic from topic provisioning process {TopicProvisioningProcessId}", processId);

        var process = await _provisioningProcessRepository.Get(processId);

        // TODO [jandr@2022-10-24]: handle cases where the topic already exists
        //_confluentGateway.Exists()

        await _confluentGateway.CreateTopic(
            clusterId: process.ClusterId, 
            topic: process.RequestedTopic,
            partition: process.Partitions,
            retention: process.Retention,
            cancellationToken: CancellationToken.None
        );

        process.RegisterThatTopicIsProvisioned();

        _logger.LogInformation("Topic {Topic} has been provisioned for capability {CapabilityRootId} in cluster {ClusterId}", 
            process.RequestedTopic, process.CapabilityRootId, process.ClusterId);
    }

    [Transactional, Outboxed]
    public async Task EnsureClusterAccess(TopicProvisionProcessId processId)
    {
        _logger.LogTrace("Ensuring cluster access for topic provisioning process {TopicProvisioningProcessId}", processId);

        var process = await _provisioningProcessRepository.Get(processId);

        var serviceAccount = await _serviceAccountRepository.FindBy(process.CapabilityRootId);
        if (serviceAccount is null)
        {
            throw new Exception("Noooooo!!!"); // TODO [jandr@2022-11-07]: change this!
        }

        var accessDefinition = await _clusterAccessDefinitionRepository.FindBy(process.ClusterId, serviceAccount.Id);
        if (accessDefinition is null)
        {
            _logger.LogTrace("Service account {ServiceAccountId} does not yet have access defined for cluster {ClusterId}", serviceAccount.Id, process.ClusterId);
            var newAccessDefinition = await _clusterAccessDomainService.DefineClusterAccessBetween(serviceAccount.Id, process.ClusterId);
            await _clusterAccessDefinitionRepository.Add(newAccessDefinition);
        }
        else
        {
            _logger.LogTrace("Capability {CapabilityRootId} (through service account {ServiceAccountId}) already has access definition {ClusterAccessDefinitionId} for cluster {ClusterId}", 
                process.CapabilityRootId, serviceAccount.Id, accessDefinition.Id, process.ClusterId);
        }
    }
}