using Dafda.Consuming;
using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.Events;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Policies;

public class WhenCapabilityHasServiceAccountThenEnsureClusterAccess : IMessageHandler<CapabilityHasServiceAccount>
{
    private readonly ILogger<WhenCapabilityHasServiceAccountThenEnsureClusterAccess> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenCapabilityHasServiceAccountThenEnsureClusterAccess(ILogger<WhenCapabilityHasServiceAccountThenEnsureClusterAccess> logger,
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
            _logger.LogError("Unable to parse a valid topic provisioning process id from \"{TopicProvisioningProcessId}\" - cannot ensure cluster access has been granted! Skipping {MessageType}", 
                message.ProcessId, context.MessageType);
            return; // skipping!
        }


        await _applicationService.EnsureClusterAccess(processId);
    }
}