using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.Tests.Builders;

public class TopicProvisioningProcessRepositoryBuilder
{
    private KafkaJanitorDbContext? _dbContext;

    public TopicProvisioningProcessRepositoryBuilder WithDbContext(KafkaJanitorDbContext? dbContext)
    {
        _dbContext = dbContext;
        return this;
    }

    public TopicProvisioningProcessRepository Build()
    {
        return new TopicProvisioningProcessRepository(_dbContext!);
    }
}