using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.Tests.Builders;

public class ServiceAccountRepositoryBuilder
{
    private KafkaJanitorDbContext? _dbContext;

    public ServiceAccountRepositoryBuilder WithDbContext(KafkaJanitorDbContext? dbContext)
    {
        _dbContext = dbContext;
        return this;
    }

    public ServiceAccountRepository Build()
    {
        return new ServiceAccountRepository(_dbContext!);
    }
}