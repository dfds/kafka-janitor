using KafkaJanitor.App.Domain.Exceptions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class ClusterRepository : IClusterRepository
{
    private readonly KafkaJanitorDbContext _dbContext;

    public ClusterRepository(KafkaJanitorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Cluster>> GetAll()
    {
        return await _dbContext.Clusters.ToListAsync();
    }

    public async Task<Cluster> Get(ClusterId clusterId)
    {
        var result = await _dbContext.Clusters.FindAsync(clusterId);

        if (result is null)
        {
            throw new DoesNotExistException($"A cluster with id \"{clusterId}\" does not exist.");
        }

        return result;
    }
}