using Dafda.Consuming;
using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.Events;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Policies;

public class WhenProvisioningProcessHasStartedThenCreateTopicInCluster : IMessageHandler<TopicProvisioningProcessHasStarted>
{
    private readonly ILogger<WhenProvisioningProcessHasStartedThenCreateTopicInCluster> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenProvisioningProcessHasStartedThenCreateTopicInCluster(ILogger<WhenProvisioningProcessHasStartedThenCreateTopicInCluster> logger, 
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
            _logger.LogError("Unable to parse a valid topic provisioning process id from \"{TopicProvisioningProcessId}\" - skipping topic creating for now!", message.ProcessId);
            return; // skipping!
        }

        await _applicationService.CreateTopicFrom(processId);

        // TODO [jandr@2022-10-14]: catch exceptions...!
    }
}