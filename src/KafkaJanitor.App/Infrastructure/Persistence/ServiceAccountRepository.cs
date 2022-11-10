using KafkaJanitor.App.Domain.Exceptions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class ServiceAccountRepository : IServiceAccountRepository
{
    private readonly KafkaJanitorDbContext _dbContext;

    public ServiceAccountRepository(KafkaJanitorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private IQueryable<ServiceAccount> ForCompleteServiceAccount() => _dbContext.ServiceAccounts
        .Include(x => x.ClusterApiKeys);

    public async Task<ServiceAccount?> FindBy(CapabilityRootId rootId)
    {
        return await ForCompleteServiceAccount()
            .SingleOrDefaultAsync(x => x.CapabilityRootId == rootId);
    }

    public async Task Add(ServiceAccount serviceAccount)
    {
        await _dbContext.ServiceAccounts.AddAsync(serviceAccount);
    }

    public async Task<ServiceAccount> Get(ServiceAccountId serviceAccountId)
    {
        var result = await ForCompleteServiceAccount()
            .SingleOrDefaultAsync(x => x.Id == serviceAccountId);

        if (result is null)
        {
            throw new DoesNotExistException($"A service account with id \"{serviceAccountId}\" does not exist.");
        }

        return result;
    }
}