using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.TestDoubles;

public class StubClusterRepository : IClusterRepository
{
    private readonly IEnumerable<Cluster> _clusters;

    public StubClusterRepository(IEnumerable<Cluster> clusters)
    {
        _clusters = clusters;
    }

    public Task<IEnumerable<Cluster>> GetAll() 
        => Task.FromResult(_clusters);

    public Task<Cluster> Get(ClusterId clusterId) 
        => Task.FromResult(_clusters.Single());
}