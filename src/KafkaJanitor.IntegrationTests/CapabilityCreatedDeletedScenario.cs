using System;
using System.Net.Http;
using System.Threading.Tasks;
using KafkaJanitor.IntegrationTests.Utils;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using Tika.RestClient;
using Tika.RestClient.Features.Acls.Models;
using Tika.RestClient.Features.ApiKeys.Models;
using Tika.RestClient.Features.ServiceAccounts.Models;
using Xunit;
using ServiceAccount = Tika.RestClient.Features.ServiceAccounts.Models.ServiceAccount;

namespace KafkaJanitor.IntegrationTests
{
    public class CapabilityCreatedDeletedScenario
    {
        private IRestClient _tikaClient;
        private Capability capability;
        private ServiceAccount serviceAccount;
        private ApiKey apiKey;
        
        [Fact]
        public async Task CapabilityCreatedDeletedScenarioRecipe()
        {
            await When_a_capability_is_created();
                  And_a_tikaClient_is_provided();
            await Then_a_service_account_is_created(capability);
            await And_acls_to_create_read_write_to_topics_under_prefix(capability, serviceAccount);
            await And_api_keypair_is_created(capability, serviceAccount);
            await When_a_capability_is_deleted();
            await Then_the_connected_acls_are_deleted(capability, serviceAccount);
            await And_connected_api_keypair_is_removed(apiKey);
            await And_the_connected_service_account_is_removed(serviceAccount);
        }

        private async Task When_a_capability_is_created()
        {
            var kafkaClient = new KafkaClient();
            capability = new Capability
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
        }
        
        private void And_a_tikaClient_is_provided()
        {
            _tikaClient = Tika.RestClient.Factories.RestClientFactory.CreateFromConfiguration(new HttpClient(), Options.Create(new ClientOptions
            {
                TIKA_API_ENDPOINT = "http://localhost:3000/"
            }));
        }

        private async Task Then_a_service_account_is_created(Capability capability)
        {
            serviceAccount = await _tikaClient.ServiceAccounts.CreateAsync(new ServiceAccountCreateCommand
            {
                name = $"{capability.Name}",
                description = "Creating with TikaService using KafkaJanitor"
            });
        }

        private async Task And_acls_to_create_read_write_to_topics_under_prefix(Capability capability, ServiceAccount serviceAccount)
        {
            // Topic
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "WRITE", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "CREATE", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "READ", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "DESCRIBE", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "DESCRIBE-CONFIGS", capability.Name));

            // ConsumerGroup
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "WRITE", "", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "CREATE", "", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, true, "READ", "", capability.Name));
            
            // DENY
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, false, "alter"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, false, "alter-configs"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, false, "cluster-action"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccount.Id, false, "create", "'*'"));
        }

        private async Task And_api_keypair_is_created(Capability capability, ServiceAccount serviceAccount)
        {
            apiKey = await _tikaClient.ApiKeys.CreateAsync(new ApiKeyCreate
            {
                Description = "Creating during CapabilityCreatedDeletedScenario",
                ServiceAccountId = serviceAccount.Id
            });
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
            // Deletion of ACL seems to be flawed if one is using Tika in notconnected mode(e.g. not calling the actual Confluent Cloud API).

            // Topic
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "WRITE", capability.Name));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "CREATE", capability.Name));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "READ", capability.Name));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "DESCRIBE", capability.Name));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "DESCRIBE-CONFIGS", capability.Name));
            
            // ConsumerGroup
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "WRITE", "", capability.Name));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "CREATE", "", capability.Name));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, true, "READ", "", capability.Name));
            
            // DENY
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, false, "alter"));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, false, "alter-configs"));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, false, "cluster-action"));
            await _tikaClient.Acls.DeleteAsync(new AclCreateDelete(serviceAccount.Id, false, "create", "'*'"));
        }

        private async Task And_connected_api_keypair_is_removed(ApiKey apiKey)
        {
            await _tikaClient.ApiKeys.DeleteAsync(apiKey.Key);
        }

        private async Task And_the_connected_service_account_is_removed(ServiceAccount serviceAccount)
        {
            await _tikaClient.ServiceAccounts.DeleteAsync(serviceAccount.Id);
        }

        public CapabilityCreatedDeletedScenario()
        {
            
        }
    }
}