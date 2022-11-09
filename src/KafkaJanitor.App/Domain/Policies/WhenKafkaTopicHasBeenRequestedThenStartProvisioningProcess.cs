using Dafda.Consuming;
using KafkaJanitor.App.BoundedContexts.Capabilities.Events;
using KafkaJanitor.App.Domain.Application;

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

    public Task Handle(KafkaTopicHasBeenRequested message, MessageHandlerContext context)
    {
        //_applicationService.StartProvisioningProcess()

        throw new NotImplementedException();
    }
}