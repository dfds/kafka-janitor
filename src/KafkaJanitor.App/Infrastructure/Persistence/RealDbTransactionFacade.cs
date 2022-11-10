using KafkaJanitor.App.Configurations;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class RealDbTransactionFacade : IDbTransactionFacade
{
    private readonly KafkaJanitorDbContext _dbContext;

    public RealDbTransactionFacade(KafkaJanitorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction()
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();

        return new RealDbTransaction(_dbContext, transaction);
    }
}