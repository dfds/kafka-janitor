﻿using Dafda.Consuming;
using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.Events;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.App.Domain.Policies;

public class WhenACLEntryHasBeenAssignedThenRegisterMissingEntries : IMessageHandler<ACLEntryHasBeenAssigned>
{
    private readonly ILogger<WhenACLEntryHasBeenAssignedThenRegisterMissingEntries> _logger;
    private readonly ITopicProvisioningApplicationService _applicationService;

    public WhenACLEntryHasBeenAssignedThenRegisterMissingEntries(ILogger<WhenACLEntryHasBeenAssignedThenRegisterMissingEntries> logger,
        ITopicProvisioningApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    public async Task Handle(ACLEntryHasBeenAssigned message, MessageHandlerContext context)
    {
        using var _ = _logger.BeginScope("{MessageType} {MessageId} {CorrelationId} {CausationId}",
            context.MessageType, context.MessageId, context.CorrelationId, context.CausationId);

        if (!ClusterAccessDefinitionId.TryParse(message.ClusterAccessDefinitionId, out var accessId))
        {
            _logger.LogError("Unable to parse a valid cluster access definition id from \"{ClusterAccessDefinitionId}\" - skipping {MessageType}!", 
                message.ClusterAccessDefinitionId, context.MessageType);

            return;
        }

        await _applicationService.ApplyNextMissingACLEntry(accessId);
    }
}