using System.Net.Http;
using System.Threading.Tasks;
using KafkaJanitor.IntegrationTests.Utils;
using KafkaJanitor.WebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Tika.Client;
using Tika.Client.Models;
using Xunit;

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
                Name = "CapabilityCreatedDeletedScenario"
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
            var tikaClient = new TikaClient(new HttpClient(), new TikaOptions(_configuration));
            return await tikaClient.CreateServiceAccount($"{capability.Name}_sa", "Creating during CapabilityCreatedDeletedScenario");
        }

        private async Task And_acls_to_create_read_write_to_topics_under_prefix(Capability capability, ServiceAccount serviceAccount)
        {
            var tikaClient = new TikaClient(new HttpClient(), new TikaOptions(_configuration));
            await tikaClient.CreateAcl(serviceAccount.Id, true, "WRITE", capability.Name, capability.Name);
        }

        private async Task When_a_capability_is_deleted()
        {
            var kafkaClient = new KafkaClient();
            var capability = new Capability
            {
                Id = "1e0a8f85-de38-42ef-a4f4-87c3b4f9a5f9",
                Name = "CapabilityCreatedDeletedScenario"
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
            var tikaClient = new TikaClient(new HttpClient(), new TikaOptions(_configuration));
            await tikaClient.DeleteAcl(serviceAccount.Id, true, "WRITE", capability.Name, capability.Name);
        }

        private async Task And_the_connected_service_account_is_removed(ServiceAccount serviceAccount)
        {
            var tikaClient = new TikaClient(new HttpClient(), new TikaOptions(_configuration));
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