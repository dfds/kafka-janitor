using System.Threading.Tasks;
using KafkaJanitor.App.Domain.Exceptions;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.Builders;
using KafkaJanitor.Tests.Comparers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KafkaJanitor.Tests.Infrastructure;

public class TestTopicProvisioningProcessRepository
{
    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task add_inserts_expected_into_database()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stub = A.TopicProvisioningProcess.Build();

        var sut = A.TopicProvisioningProcessRepository
            .WithDbContext(dbContext)
            .Build();

        await sut.Add(stub);
        await dbContext.SaveChangesAsync();

        var inserted = Assert.Single(await dbContext.TopicProvisioningProcesses.ToListAsync());
        Assert.Equal(stub, inserted, new TopicProvisioningProcessComparer());
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_returns_expected_from_database()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stub = A.TopicProvisioningProcess.Build();

        await dbContext.TopicProvisioningProcesses.AddAsync(stub);
        await dbContext.SaveChangesAsync();

        var sut = A.TopicProvisioningProcessRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstance = await sut.Get(stub.Id);

        Assert.Equal(stub, readInstance, new TopicProvisioningProcessComparer());
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task get_throws_expected_exception_when_nothing_was_found()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var sut = A.TopicProvisioningProcessRepository
            .WithDbContext(dbContext)
            .Build();

        await Assert.ThrowsAsync<DoesNotExistException>(() => sut.Get(TopicProvisionProcessId.None));
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task find_by_capability_root_id_returns_expected_when_nothing_could_be_found()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var sut = A.TopicProvisioningProcessRepository
            .WithDbContext(dbContext)
            .Build();

        var result = await sut.FindAllActiveBy(CapabilityRootId.Parse("foo"));

        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task find_by_capability_root_id_returns_expected_on_successful_match()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stubExpectedProcess = A.TopicProvisioningProcess
            .WithCapabilityRootId(CapabilityRootId.Parse("foo"))
            .WithIsCompleted(false)
            .Build();

        await dbContext.TopicProvisioningProcesses.AddRangeAsync(new[]
        {
            A.TopicProvisioningProcess
                .WithCapabilityRootId(CapabilityRootId.Parse("bar"))
                .WithIsCompleted(false)
                .Build(),
            A.TopicProvisioningProcess
                .WithCapabilityRootId(CapabilityRootId.Parse("baz"))
                .WithIsCompleted(false)
                .Build(),
            A.TopicProvisioningProcess
                .WithCapabilityRootId(CapabilityRootId.Parse("qux"))
                .WithIsCompleted(true)
                .Build(),
        });
        await dbContext.TopicProvisioningProcesses.AddAsync(stubExpectedProcess);
        await dbContext.SaveChangesAsync();

        var sut = A.TopicProvisioningProcessRepository
            .WithDbContext(dbContext)
            .Build();

        var readInstance = await sut.FindAllActiveBy(stubExpectedProcess.CapabilityRootId);

        Assert.Equal(new []{ stubExpectedProcess }, readInstance, new TopicProvisioningProcessComparer());
    }
}