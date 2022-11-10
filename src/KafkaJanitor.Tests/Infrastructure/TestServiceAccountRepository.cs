using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Exceptions;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.Builders;
using KafkaJanitor.Tests.Comparers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KafkaJanitor.Tests.Infrastructure;

public class TestServiceAccountRepository
{
    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task add_inserts_expected_into_database()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stub = new ServiceAccountBuilder().Build();

        var sut = new ServiceAccountRepositoryBuilder()
            .WithDbContext(dbContext)
            .Build();

        await sut.Add(stub);
        await dbContext.SaveChangesAsync();

        var inserted = Assert.Single(await dbContext.ServiceAccounts.ToListAsync());
        Assert.Equal(stub, inserted, new ServiceAccountSemanticComparer());
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task add_also_inserts_expected_apikeys_into_database()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stubClusterApiKey = A.ClusterApiKey;

        var stub = new ServiceAccountBuilder()
            .WithClusterApiKeys(stubClusterApiKey)
            .Build();

        // guard assert
        Assert.NotEmpty(stub.ClusterApiKeys);

        var sut = A.ServiceAccountRepository
            .WithDbContext(dbContext)
            .Build();

        await sut.Add(stub);
        await dbContext.SaveChangesAsync();

        var inserted = Assert.Single(await dbContext.ServiceAccounts.ToListAsync());

        Assert.NotEmpty(inserted.ClusterApiKeys);
        Assert.Equal(stub.ClusterApiKeys, inserted.ClusterApiKeys);
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_returns_expected_from_database()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stub = new ServiceAccountBuilder().Build();

        await dbContext.ServiceAccounts.AddAsync(stub);
        await dbContext.SaveChangesAsync();

        var sut = A.ServiceAccountRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstance = await sut.Get(stub.Id);

        Assert.Equal(stub, readInstance, new ServiceAccountSemanticComparer());
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_throws_expected_exception_when_nothing_could_be_found()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var sut = A.ServiceAccountRepository
            .WithDbContext(dbContext)
            .Build();

        await Assert.ThrowsAsync<DoesNotExistException>(() => sut.Get(ServiceAccountId.Parse("foo")));
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task find_by_capability_root_id_returns_expected_when_nothing_could_be_found()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var sut = A.ServiceAccountRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstance = await sut.FindBy(CapabilityRootId.Parse("foo"));

        Assert.Null(readInstance);
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task find_by_capability_root_id_returns_expected_on_successful_match()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stubCapabilityRootId = CapabilityRootId.Parse("foo");

        var stub = new ServiceAccountBuilder()
            .WithCapabilityRootId(stubCapabilityRootId)
            .Build();

        await dbContext.ServiceAccounts.AddAsync(stub);
        await dbContext.SaveChangesAsync();

        var sut = A.ServiceAccountRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstance = await sut.FindBy(stubCapabilityRootId);

        Assert.Equal(stub, readInstance, new ServiceAccountSemanticComparer());
    }
}