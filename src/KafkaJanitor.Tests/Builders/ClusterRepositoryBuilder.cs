using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.Tests.Builders;

public class ClusterRepositoryBuilder
{
    private KafkaJanitorDbContext? _dbContext;

    public ClusterRepositoryBuilder WithDbContext(KafkaJanitorDbContext? dbContext)
    {
        _dbContext = dbContext;
        return this;
    }

    public ClusterRepository Build()
    {
        return new ClusterRepository(_dbContext!);
    }
}