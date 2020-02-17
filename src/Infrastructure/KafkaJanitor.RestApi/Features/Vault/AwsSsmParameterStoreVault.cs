using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class AwsSsmParameterStoreVault : IVault
    {
        public AwsSsmParameterStoreVault()
        {
        }
        public async Task AddApiCredentials(ApiCredentials apiCredentials)
        {
            var ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.EUCentral1);
            await ssmClient.PutParameterAsync(new PutParameterRequest
            {
                Type = ParameterType.SecureString,
                Name = "/managed/deploy/kafka-creds",
                Tier = ParameterTier.Standard,
                Value = $"KEY: {apiCredentials.Key}\nSECRET: {apiCredentials.Secret}"
            });
        }
    }
}