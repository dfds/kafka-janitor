using Dafda.Outbox;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class KafkaJanitorDbContext : DbContext
{
    public KafkaJanitorDbContext(DbContextOptions<KafkaJanitorDbContext> options) : base(options)
    {

    }

    public DbSet<OutboxEntry> OutboxEntries { get; set; } = null!;

    public DbSet<ServiceAccount> ServiceAccounts { get; set; } = null!;
    public DbSet<TopicProvisioningProcess> TopicProvisioningProcesses { get; set; } = null!;
    public DbSet<Cluster> Clusters { get; set; } = null!;
    public DbSet<ClusterAccessDefinition> ClusterAccessDefinitions { get; set; } = null!;

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
            .Properties<TopicPartition>()
            .HaveConversion<TopicPartitionConverter>();

        configurationBuilder
            .Properties<ClusterApiKeyId>()
            .HaveConversion<ClusterApiKeyIdConverter>();

        configurationBuilder
            .Properties<AccessControlListEntryId>()
            .HaveConversion<AccessControlListEntryIdConverter>();

        configurationBuilder
            .Properties<ClusterId>()
            .HaveConversion<ClusterIdConverter>();

        configurationBuilder
            .Properties<ClusterAccessDefinitionId>()
            .HaveConversion<ClusterAccessDefinitionIdConverter>();

        configurationBuilder
            .Properties<ACLEntryOperationType>()
            .HaveConversion<string>();

        configurationBuilder
            .Properties<ACLEntryPatternType>()
            .HaveConversion<string>();

        configurationBuilder
            .Properties<ACLEntryPermissionType>()
            .HaveConversion<string>();

        configurationBuilder
            .Properties<ACLEntryResourceType>()
            .HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OutboxEntry>(cfg =>
        {
            cfg.ToTable("_outbox");
            cfg.HasKey(x => x.MessageId);
            cfg.Property(x => x.MessageId).HasColumnName("Id");
            cfg.Property(x => x.Topic);
            cfg.Property(x => x.Key);
            cfg.Property(x => x.Payload);
            cfg.Property(x => x.OccurredUtc);
            cfg.Property(x => x.ProcessedUtc);
        });

        modelBuilder.Entity<ServiceAccount>(cfg =>
        {
            cfg.ToTable("ServiceAccount");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.CapabilityRootId);
            cfg.HasMany(x => x.ClusterApiKeys);
        });

        modelBuilder.Entity<AccessControlListEntry>(cfg =>
        {
            cfg.ToTable("AccessControlListEntry");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.IsApplied);
            
            cfg.OwnsOne(x => x.Descriptor, y =>
            {
                y.Property(x => x.OperationType);
                y.Property(x => x.PatternType);
                y.Property(x => x.PermissionType);
                y.Property(x => x.ResourceName);
                y.Property(x => x.ResourceType);
            });
        });

        modelBuilder.Entity<TopicProvisioningProcess>(cfg =>
        {
            cfg.ToTable("TopicProvisioningProcess");
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.RequestedTopic);
            cfg.Property(x => x.ClusterId);
            cfg.Property(x => x.Partitions);
            cfg.Property(x => x.Retention);

            cfg.Property(x => x.CapabilityRootId);
            
            cfg.Property(x => x.IsServiceAccountCreated);
            cfg.Property(x => x.IsServiceAccountGrantedAccessToCluster);
            cfg.Property(x => x.IsTopicProvisioned);
            cfg.Property(x => x.IsApiKeyStoredInVault);
            cfg.Property(x => x.IsCompleted);
        });

        modelBuilder.Entity<Cluster>(cfg =>
        {
            cfg.ToTable("Cluster");
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.Name);
            cfg.Property(x => x.BootstrapEndpoint);
            cfg.Property(x => x.AdminApiEndpoint);
        });

        modelBuilder.Entity<ClusterAccessDefinition>(cfg =>
        {
            cfg.ToTable("ClusterAccessDefinition");
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.ClusterId);
            cfg.Property(x => x.ServiceAccountId);
            cfg.HasMany(x => x.AccessControlList);
        });
    }
}
