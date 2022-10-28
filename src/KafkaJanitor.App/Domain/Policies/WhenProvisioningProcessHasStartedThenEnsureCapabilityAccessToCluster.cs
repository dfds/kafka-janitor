using Dafda.Consuming;
using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.Events;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Policies;

public class WhenProvisioningProcessHasStartedThenEnsureCapabilityAccessToCluster : IMessageHandler<TopicProvisioningProcessHasStarted>
{
    private readonly ILogger<WhenProvisioningProcessHasStartedThenEnsureCapabilityAccessToCluster> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenProvisioningProcessHasStartedThenEnsureCapabilityAccessToCluster(ILogger<WhenProvisioningProcessHasStartedThenEnsureCapabilityAccessToCluster> logger,
        ITopicProvisioningApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    public async Task Handle(TopicProvisioningProcessHasStarted message, MessageHandlerContext context)
    {
        using var _ = _logger.BeginScope("{MessageType} {MessageId} {CorrelationId} {CausationId}",
            context.MessageType, context.MessageId, context.CorrelationId, context.CausationId);

        if (!TopicProvisionProcessId.TryParse(message.ProcessId, out var processId))
        {
            _logger.LogError("Unable to parse a valid topic provisioning process id from \"{TopicProvisioningProcessId}\" - cannot ensure access has been granted!", message.ProcessId);
            return; // skipping!
        }

        await _applicationService.CreateServiceAccount(processId);

        // TODO [jandr@2022-10-24]: catch exceptions...!
    }
}