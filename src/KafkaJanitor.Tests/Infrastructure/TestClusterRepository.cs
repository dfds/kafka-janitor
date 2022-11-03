using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Exceptions;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.Builders;
using KafkaJanitor.Tests.Comparers;
using Xunit;

namespace KafkaJanitor.Tests.Infrastructure;

public class TestClusterRepository
{
    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_all_returns_expected_from_database_when_empty()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var sut = A.ClusterRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstances = await sut.GetAll();

        Assert.Empty(readInstances);
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_all_returns_expected_from_database_when_having_single()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stub = A.Cluster.Build();

        await dbContext.Clusters.AddAsync(stub);
        await dbContext.SaveChangesAsync();

        var sut = A.ClusterRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstances = await sut.GetAll();

        Assert.Equal(new[]{ stub }, readInstances, new ClusterSemanticComparer());
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_all_returns_expected_from_database_when_having_multiple()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stubs = new[]
        {
            A.Cluster
                .WithId(ClusterId.Parse("foo"))
                .WithName("foo")
                .Build(),
            A.Cluster
                .WithId(ClusterId.Parse("bar"))
                .WithName("bar")
                .Build(),
        };

        await dbContext.Clusters.AddRangeAsync(stubs);
        await dbContext.SaveChangesAsync();

        var sut = A.ClusterRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstances = await sut.GetAll();

        Assert.Equal(
            expected: stubs.OrderBy(x => x.Id).ToArray(),
            actual: readInstances.OrderBy(x => x.Id).ToArray(),
            comparer: new ClusterSemanticComparer()
        );
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_returns_expected_from_database()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stub = A.Cluster.Build();

        await dbContext.Clusters.AddAsync(stub);
        await dbContext.SaveChangesAsync();

        var sut = A.ClusterRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstance = await sut.Get(stub.Id);

        Assert.Equal(stub, readInstance, new ClusterSemanticComparer());
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_throws_expected_exception_when_nothing_could_be_found()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var sut = A.ClusterRepository
            .WithDbContext(dbContext)
            .Build();

        await Assert.ThrowsAsync<DoesNotExistException>(() => sut.Get(ClusterId.None));
    }
}