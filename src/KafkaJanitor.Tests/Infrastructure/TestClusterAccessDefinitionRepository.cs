using System.Threading.Tasks;
using KafkaJanitor.Tests.Builders;
using KafkaJanitor.Tests.Comparers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KafkaJanitor.Tests.Infrastructure;

public class TestClusterAccessDefinitionRepository
{
    [Fact]
    [Trait("Category", "InMemoryDatabase")]
    public async Task add_inserts_expected_into_database()
    {
        await using var databaseFactory = new InMemoryDatabaseFactory();
        var dbContext = await databaseFactory.CreateDbContext();

        var stub = A.ClusterAccessDefinition.Build();

        var sut = A.ClusterAccessDefinitionRepository
            .WithDbContext(dbContext)
            .Build();

        await sut.Add(stub);
        await dbContext.SaveChangesAsync();

        var inserted = Assert.Single(await dbContext.ClusterAccessDefinitions.ToListAsync());
        Assert.Equal(stub, inserted, new ClusterAccessDefinitionSemanticComparer());
    }
}