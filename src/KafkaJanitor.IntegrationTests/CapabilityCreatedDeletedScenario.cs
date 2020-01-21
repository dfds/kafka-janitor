using System.Net.Http;
using System.Threading.Tasks;
using KafkaJanitor.IntegrationTests.Utils;
using KafkaJanitor.WebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using Tika.Client;
using Tika.Client.Models;
using Xunit;
using ServiceAccount = Tika.Client.Models.ServiceAccount;

namespace KafkaJanitor.IntegrationTests
{
    public class CapabilityCreatedDeletedScenario
    {
        private IConfiguration _configuration;
        
        [Fact]
        public async Task CapabilityCreatedDeletedScenarioRecipe()
        {
            var capability = await When_a_capability_is_created();
            var serviceAccount = await Then_a_service_account_is_created(capability);
            await And_acls_to_create_read_write_to_topics_under_prefix(capability, serviceAccount);
            await When_a_capability_is_deleted();
            await Then_the_connected_acls_are_deleted(capability, serviceAccount);
            await And_the_connected_service_account_is_removed(serviceAccount);
        }

        private async Task<Capability> When_a_capability_is_created()
        {
            var kafkaClient = new KafkaClient();
            var capability = new Capability
            {
                Id = "1e0a8f85-de38-42ef-a4f4-87c3b4f9a5f9",
                Name = "devx-acltest4"
            };

            var message = new
            {
                CapabilityId = capability.Id,
                CapabilityName = capability.Name
            };

            await kafkaClient.SendMessageAsync(message, "capability_created");
            return capability;
        }

        private async Task<ServiceAccount> Then_a_service_account_is_created(Capability capability)
        {
            var tikaClient = new TikaClient(new HttpClient(), Options.Create(new TikaOptions(_configuration)));
            return await tikaClient.CreateServiceAccount($"{capability.Name}_sa", "Creating during CapabilityCreatedDeletedScenario");
        }

        private async Task And_acls_to_create_read_write_to_topics_under_prefix(Capability capability, ServiceAccount serviceAccount)
        {
            var tikaClient = new TikaClient(new HttpClient(), Options.Create(new TikaOptions(_configuration)));
            
            // Topic
            await tikaClient.CreateAcl(serviceAccount.Id, true, "WRITE", capability.Name);
            await tikaClient.CreateAcl(serviceAccount.Id, true, "CREATE", capability.Name);
            await tikaClient.CreateAcl(serviceAccount.Id, true, "READ", capability.Name);
            await tikaClient.CreateAcl(serviceAccount.Id, true, "DESCRIBE", capability.Name);
            await tikaClient.CreateAcl(serviceAccount.Id, true, "DESCRIBE-CONFIGS", capability.Name);
            
            // ConsumerGroup
            await tikaClient.CreateAcl(serviceAccount.Id, true, "WRITE", "", capability.Name);
            await tikaClient.CreateAcl(serviceAccount.Id, true, "CREATE", "", capability.Name);
            await tikaClient.CreateAcl(serviceAccount.Id, true, "READ", "", capability.Name);
            
            // DENY
            await tikaClient.CreateAcl(serviceAccount.Id, false, "alter");
            await tikaClient.CreateAcl(serviceAccount.Id, false, "alter-configs");
            await tikaClient.CreateAcl(serviceAccount.Id, false, "cluster-action");
            await tikaClient.CreateAcl(serviceAccount.Id, false, "create", "'*'");
        }

        private async Task When_a_capability_is_deleted()
        {
            var kafkaClient = new KafkaClient();
            var capability = new Capability
            {
                Id = "1e0a8f85-de38-42ef-a4f4-87c3b4f9a5f9",
                Name = "devx-acltest4"
            };

            var message = new
            {
                CapabilityId = capability.Id,
                CapabilityName = capability.Name
            };

            await kafkaClient.SendMessageAsync(message, "capability_deleted");
        }

        private async Task Then_the_connected_acls_are_deleted(Capability capability, ServiceAccount serviceAccount)
        {
            var tikaClient = new TikaClient(new HttpClient(), Options.Create(new TikaOptions(_configuration)));
            
            // Topic
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "WRITE", capability.Name);
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "CREATE", capability.Name);
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "READ", capability.Name);
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "DESCRIBE", capability.Name);
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "DESCRIBE-CONFIGS", capability.Name);
            
            // ConsumerGroup
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "WRITE", "", capability.Name);
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "CREATE", "", capability.Name);
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "READ", "", capability.Name);
            
            // DENY
            await tikaClient.DeleteAcl(serviceAccount.Id, false, "alter");
            await tikaClient.DeleteAcl(serviceAccount.Id, false, "alter-configs");
            await tikaClient.DeleteAcl(serviceAccount.Id, false, "cluster-action");
            await tikaClient.DeleteAcl(serviceAccount.Id, false, "create", "'*'");
        }

        private async Task And_the_connected_service_account_is_removed(ServiceAccount serviceAccount)
        {
            var tikaClient = new TikaClient(new HttpClient(), Options.Create(new TikaOptions(_configuration)));
            await tikaClient.DeleteServiceAccount(serviceAccount.Id);
        }

        public CapabilityCreatedDeletedScenario()
        {
            _configuration = new ConfigurationBuilder()
                .Add(new EnvironmentVariablesConfigurationSource())
                .Build();
            
        }
    }
}