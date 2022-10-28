using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class KafkaJanitorDbContext : DbContext
{
    public KafkaJanitorDbContext(DbContextOptions<KafkaJanitorDbContext> options) : base(options)
    {

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<TopicProvisionProcessId>()
            .HaveConversion<TopicProvisioningProcessIdConverter>();

        configurationBuilder
            .Properties<TopicName>()
            .HaveConversion<TopicNameConverter>();

        configurationBuilder
            .Properties<TopicRetention>()
            .HaveConversion<TopicRetentionConverter>();

        configurationBuilder
            .Properties<ServiceAccountId>()
            .HaveConversion<ServiceAccountIdConverter>();

        configurationBuilder
            .Properties<CapabilityRootId>()
            .HaveConversion<CapabilityRootIdConverter>();

        configurationBuilder
            .Properties<GrantCapabilityAccessProcessId>()
            .HaveConversion<GrantCapabilityAccessProcessIdConverter>();

        configurationBuilder
            .Properties<TopicPartition>()
            .HaveConversion<TopicPartitionConverter>();

        configurationBuilder
            .Properties<ClusterApiKeyId>()
            .HaveConversion<ClusterApiKeyIdConverter>();
    }
}