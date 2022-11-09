using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.Tests.Builders;

public class ClusterAccessDefinitionRepositoryBuilder
{
    private KafkaJanitorDbContext? _dbContext;

    public ClusterAccessDefinitionRepositoryBuilder WithDbContext(KafkaJanitorDbContext? dbContext)
    {
        _dbContext = dbContext;
        return this;
    }

    public ClusterAccessDefinitionRepository Build()
    {
        return new ClusterAccessDefinitionRepository(_dbContext!);
    }
}