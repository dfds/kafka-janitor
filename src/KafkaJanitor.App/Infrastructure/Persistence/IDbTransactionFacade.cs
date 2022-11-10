using KafkaJanitor.App.Configurations;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public interface IDbTransactionFacade
{
    Task<IDbTransaction> BeginTransaction();
}