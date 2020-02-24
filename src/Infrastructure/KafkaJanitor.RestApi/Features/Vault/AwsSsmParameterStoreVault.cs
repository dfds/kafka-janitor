using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class AwsSsmParameterStoreVault : IVault
    {
        public async Task AddApiCredentials(Capability capability, ApiCredentials apiCredentials)
        {
            var ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.EUCentral1);
            await ssmClient.PutParameterAsync(new PutParameterRequest
            {
                Type = ParameterType.SecureString,
                Name = $"/capabilities/{capability.Id}/kafka/credentials",
                Tier = ParameterTier.Standard,
                Value = $"KEY: {apiCredentials.Key}\nSECRET: {apiCredentials.Secret}",
                Tags = new List<Tag>
                {
                    new Tag{Key = "capabilityName",Value = capability.Name},
                    new Tag{Key = "capabilityId",Value = capability.Id}
                }
            });
        }
    }
}