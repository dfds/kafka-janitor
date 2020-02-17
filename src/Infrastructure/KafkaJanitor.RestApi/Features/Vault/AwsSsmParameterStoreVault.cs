using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class AwsSsmParameterStoreVault : IVault
    {
        private readonly AmazonSecurityTokenServiceClient _amazonSecurityTokenServiceClient;
        public AwsSsmParameterStoreVault()
        {
            _amazonSecurityTokenServiceClient = new AmazonSecurityTokenServiceClient();
            var caller = _amazonSecurityTokenServiceClient.GetCallerIdentityAsync(new GetCallerIdentityRequest()).Result;
            Console.WriteLine($"Caller identity: {caller.Arn} | {caller.Account} | {caller.UserId}");
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