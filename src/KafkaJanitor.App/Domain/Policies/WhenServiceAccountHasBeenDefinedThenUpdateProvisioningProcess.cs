using Dafda.Consuming;
using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.Events;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Policies;

public class WhenServiceAccountHasBeenDefinedThenUpdateProvisioningProcess : IMessageHandler<ServiceAccountHasBeenDefined>
{
    private readonly ILogger<WhenServiceAccountHasBeenDefinedThenUpdateProvisioningProcess> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenServiceAccountHasBeenDefinedThenUpdateProvisioningProcess(ILogger<WhenServiceAccountHasBeenDefinedThenUpdateProvisioningProcess> logger,
        ITopicProvisioningApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    public async Task Handle(ServiceAccountHasBeenDefined message, MessageHandlerContext context)
    {
        using var _ = _logger.BeginScope("{MessageType} {MessageId} {CorrelationId} {CausationId}",
            context.MessageType, context.MessageId, context.CorrelationId, context.CausationId);

        if (!ServiceAccountId.TryParse(message.ServiceAccountId, out var serviceAccountId))
        {
            _logger.LogError("Unable to parse a valid service account id from \"{ServiceAccountId}\" - skipping {MessageType}!", 
                message.ServiceAccountId, context.MessageType);

            return;
        }

        await _applicationService.RegisterNewServiceAccountIsDefined(serviceAccountId);
    }
}

public class WhenCapabilityHasServiceAccountThenAssignApiKeyToServiceAccount : IMessageHandler<CapabilityHasServiceAccount>
{
    private readonly ILogger<WhenCapabilityHasServiceAccountThenAssignApiKeyToServiceAccount> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenCapabilityHasServiceAccountThenAssignApiKeyToServiceAccount(ILogger<WhenCapabilityHasServiceAccountThenAssignApiKeyToServiceAccount> logger,
        ITopicProvisioningApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    public async Task Handle(CapabilityHasServiceAccount message, MessageHandlerContext context)
    {
        using var _ = _logger.BeginScope("{MessageType} {MessageId} {CorrelationId} {CausationId}",
            context.MessageType, context.MessageId, context.CorrelationId, context.CausationId);

        if (!TopicProvisionProcessId.TryParse(message.ProcessId, out var processId))
        {
            _logger.LogError("Unable to parse a valid topic provisioning process id from \"{TopicProvisioningProcessId}\" - skipping {MessageType}!",
                message.ProcessId, context.MessageType);

            return;
        }

        await _applicationService.EnsureServiceAccountHasApiKey(processId);
    }
}
