using Dafda.Consuming;
using KafkaJanitor.App.BoundedContexts.Capabilities.Events;
using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Policies;

public class WhenKafkaTopicHasBeenRequestedThenStartProvisioningProcess : IMessageHandler<KafkaTopicHasBeenRequested>
{
    private readonly ILogger<WhenKafkaTopicHasBeenRequestedThenStartProvisioningProcess> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenKafkaTopicHasBeenRequestedThenStartProvisioningProcess(ILogger<WhenKafkaTopicHasBeenRequestedThenStartProvisioningProcess> logger,
        ITopicProvisioningApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    public async Task Handle(KafkaTopicHasBeenRequested message, MessageHandlerContext context)
    {
        using var _ = _logger.BeginScope("{MessageType} {MessageId} {CorrelationId} {CausationId}",
            context.MessageType, context.MessageId, context.CorrelationId, context.CausationId);

        if (!ClusterId.TryParse(message.ClusterId, out var clusterId))
        {
            _logger.LogError("Unable to parse valid cluster id from \"{ClusterId}\" and cannot start provisioning process - skipping {MessageType}", 
                message.ClusterId, context.MessageType);
            return;
        }

        if (!CapabilityRootId.TryParse(message.CapabilityRootId, out var capabilityRootId))
        {
            _logger.LogError("Unable to parse valid capability root id from \"{CapabilityRootId}\" and cannot start provisioning process - skipping {MessageType}",
                message.CapabilityRootId, context.MessageType);
            return;
        }

        if (!TopicPartition.TryParse(message.Partitions?.ToString(), out var partitions))
        {
            _logger.LogError("Unable to parse valid topic partition from \"{TopicPartition}\" and cannot start provisioning process - skipping {MessageType}",
                message.Partitions, context.MessageType);
            return;
        }

        if (!TopicRetention.TryParse(message.Retention?.ToString(), out var retention))
        {
            _logger.LogError("Unable to parse valid topic retention from \"{TopicRetention}\" and cannot start provisioning process - skipping {MessageType}",
                message.Retention, context.MessageType);
            return;
        }

        if (!TopicName.TryParse(message.TopicName, out var topicName))
        {
            _logger.LogError("Unable to parse valid topic name from \"{TopicName}\" and cannot start provisioning process - skipping {MessageType}",
                message.TopicName, context.MessageType);
            return;
        }

        var processId = await _applicationService.StartProvisioningProcess(topicName, clusterId, partitions, retention);
        _logger.LogInformation("A topic provisioning process with id {TopicProvisioningProcess} for topic {TopicName} has been started", processId, topicName);
    }
}