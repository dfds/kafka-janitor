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
}