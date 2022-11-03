using Dafda.Consuming;
using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.Events;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Policies;

public class WhenClusterApiKeyHasBeenAssignedThenAssignMissingApiKeyToServiceAccount : IMessageHandler<ClusterApiKeyHasBeenAssigned>
{
    private readonly ILogger<WhenClusterApiKeyHasBeenAssignedThenAssignMissingApiKeyToServiceAccount> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenClusterApiKeyHasBeenAssignedThenAssignMissingApiKeyToServiceAccount(ILogger<WhenClusterApiKeyHasBeenAssignedThenAssignMissingApiKeyToServiceAccount> logger, ITopicProvisioningApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    public async Task Handle(ClusterApiKeyHasBeenAssigned message, MessageHandlerContext context)
    {
        using var _ = _logger.BeginScope("{MessageType} {MessageId} {CorrelationId} {CausationId}",
            context.MessageType, context.MessageId, context.CorrelationId, context.CausationId);

        if (!ServiceAccountId.TryParse(message.ServiceAccountId, out var serviceAccountId))
        {
            _logger.LogError("Unable to parse valid service account id from {ServiceAccountId} - skipping {MessageType}", 
                message.ServiceAccountId, context.MessageType);

            return;
        }

        await _applicationService.AssignNextMissingApiKeyForServiceAccount(serviceAccountId);
    }
}