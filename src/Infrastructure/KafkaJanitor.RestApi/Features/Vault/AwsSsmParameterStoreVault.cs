using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;
using Newtonsoft.Json;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class AwsSsmParameterStoreVault : IVault
    {
        public async Task EnsureConnection()
        {
            var ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.EUCentral1);
            await ssmClient.PutParameterAsync(new PutParameterRequest
            {
                Type = ParameterType.SecureString,
                Name = $"/capabilities/HEALTH_CHECK_DUMMY/kafka/credentials",
                Tier = ParameterTier.Standard,
                Overwrite = true,
                Value = JsonConvert.SerializeObject(new
                {
                    Key = "DUMMY",
                    Secret = "DUMMY"
                })
            });
        }
        public async Task AddApiCredentials(Capability capability, ApiCredentials apiCredentials)
        {
            var ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.EUCentral1);

            await ssmClient.PutParameterAsync(new PutParameterRequest
            {
                Type = ParameterType.SecureString,
                Name = $"/capabilities/{capability.RootId}/kafka/credentials",
                Tier = ParameterTier.Standard,
                Value = JsonConvert.SerializeObject(new
                {
                    Key = apiCredentials.Key,
                    Secret = apiCredentials.Secret
                }),
                Tags = new List<Tag>
                {
                    new Tag{Key = "capabilityName",Value = capability.Name},
                    new Tag{Key = "capabilityId",Value = capability.Id},
                    new Tag{Key = "capabilityRootId",Value = capability.RootId}
                }
            });
        }
    }
}