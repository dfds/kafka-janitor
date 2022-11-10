using KafkaJanitor.App.Domain.Application;
using KafkaJanitor.App.Domain.DomainServices;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.ConfluentCloud;
using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.App.Configurations;

public static class Domain
{
    public static void ConfigureDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITopicProvisioningApplicationService, TopicProvisioningApplicationService>();
        builder.Services.AddTransient<ITopicProvisioningProcessRepository, TopicProvisioningProcessRepository>();
        
        builder.Services.AddTransient<IServiceAccountRepository, ServiceAccountRepository>();
        builder.Services.AddTransient<IClusterRepository, ClusterRepository>();
        
        builder.Services.AddTransient<IClusterAccessDefinitionRepository, ClusterAccessDefinitionRepository>();
        builder.Services.AddTransient<ClusterAccessDomainService>();
        builder.Services.AddTransient<IPasswordVaultGateway, PasswordVaultGateway>();
        builder.Services.AddHttpClient<IConfluentGateway, ConfluentGateway>();

        builder.Services.AddTransient<IConfluentCredentialsProvider, JsonBasedConfluentCredentialsProvider>();

        builder.Services.AddTransient<ITextContentReader, FileBasedTextContentReader>(provider =>
        {
            var filePath = builder.Configuration["CONFLUENT_CREDENTIALS_FILEPATH"];
            return new FileBasedTextContentReader(new FileInfo(filePath));
        });
    }
}