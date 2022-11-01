using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class KafkaJanitorDbContext : DbContext
{
    public KafkaJanitorDbContext(DbContextOptions<KafkaJanitorDbContext> options) : base(options)
    {

    }

    public DbSet<ServiceAccount> ServiceAccounts { get; set; } = null!;
    public DbSet<TopicProvisioningProcess> TopicProvisioningProcesses { get; set; } = null!;
    public DbSet<Cluster> Clusters { get; set; } = null!;

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

        configurationBuilder
            .Properties<AccessControlListEntryId>()
            .HaveConversion<AccessControlListEntryIdConverter>();

        configurationBuilder
            .Properties<ClusterId>()
            .HaveConversion<ClusterIdConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ServiceAccount>(cfg =>
        {
            cfg.ToTable("ServiceAccount");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.CapabilityRootId);
            cfg.HasMany(x => x.ClusterApiKeys);
            cfg.HasMany(x => x.AccessControlList);
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
            cfg.Property(x => x.IsServiceAccountGrantedAccess);
            cfg.Property(x => x.IsTopicProvisioned);
            cfg.Property(x => x.IsApiKeysCreated);
            cfg.Property(x => x.AreAllApiKeysStoredInVault);
            cfg.Property(x => x.IsCompleted);
        });

        modelBuilder.Entity<Cluster>(cfg =>
        {
            cfg.ToTable("Cluster");
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.Name);
            cfg.Property(x => x.BootstrapEndpoint);
        });
    }
}