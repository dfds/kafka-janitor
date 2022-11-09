using KafkaJanitor.App.Domain.Exceptions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class ClusterAccessDefinitionRepository : IClusterAccessDefinitionRepository
{
    private readonly KafkaJanitorDbContext _dbContext;

    public ClusterAccessDefinitionRepository(KafkaJanitorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(ClusterAccessDefinition definition)
    {
        await _dbContext.ClusterAccessDefinitions.AddAsync(definition);
    }

    public async Task<ClusterAccessDefinition?> FindBy(ClusterId clusterId, ServiceAccountId serviceAccountId)
    {
        return await _dbContext.ClusterAccessDefinitions
            .Where(x => x.Cluster == clusterId && x.ServiceAccount == serviceAccountId)
            .SingleOrDefaultAsync();
    }

    public async Task<ClusterAccessDefinition> Get(ClusterAccessDefinitionId id)
    {
        var result = await _dbContext.ClusterAccessDefinitions.FindAsync(id);

        if (result is null)
        {
            throw new DoesNotExistException($"Cluster access definition with id \"{id}\" does not exist.");
        }

        return result;
    }
}