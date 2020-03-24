using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.AccessControlLists.Infrastructure;
using KafkaJanitor.RestApi.Features.ApiKeys;
using KafkaJanitor.RestApi.Features.ServiceAccounts.Infrastructure;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault;
using KafkaJanitor.RestApi.Features.Vault.Model;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Features.Access.Application
{
    public class AccessService : IAccessService
    {
        private readonly IAccessControlListClient _accessControlListService;
        private readonly IServiceAccountClient _serviceAccountClient;
        private readonly IApiKeyClient _apiKeyClient;
        private readonly IVault _vault;

        public AccessService(
            IAccessControlListClient accessControlListService,
            IServiceAccountClient serviceAccountClient,
            IApiKeyClient apiKeyClient,
            IVault vault
        )
        {
            _accessControlListService = accessControlListService;
            _serviceAccountClient = serviceAccountClient;
            _apiKeyClient = apiKeyClient;
            _vault = vault;
        }


        public async Task ProvideAccess(
            Capability capability,
            string topicPrefix
        )
        {
            ServiceAccount serviceAccount;
            if (!await ServiceAccountExists(capability))
            {
                serviceAccount = await _serviceAccountClient.CreateServiceAccount(capability);
            }
            else
            {
                serviceAccount = await _serviceAccountClient.GetServiceAccount(capability);
            }

            if (!await ExpectedAmountOfAclsAreInPlace(serviceAccount.Id))
            {
                await _accessControlListService.CreateAclsForServiceAccount(serviceAccount.Id, topicPrefix);
            }

            if (!await AtLestOneApiKeyExists(serviceAccount.Id))
            {
                await CreateAndStoreApiKeyPair(
                    capability, 
                    serviceAccount
                );
            }
        }

        public async Task CreateAndStoreApiKeyPair(
            Capability cap, 
            ServiceAccount serviceAccount
        )
        {
            var apiKeyPair = await _apiKeyClient.CreateApiKeyPair(serviceAccount);

            await _vault.AddApiCredentials(
                cap,
                new ApiCredentials
                {
                    Key = apiKeyPair.Key,
                    Secret = apiKeyPair.Secret
                }
            );
        }
        
        public async Task<bool> ServiceAccountExists(Capability capability)
        {
            try
            {
                var sa = await _serviceAccountClient.GetServiceAccount(capability);
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ExpectedAmountOfAclsAreInPlace(string serviceAccountId)
        {
            var acls = await _accessControlListService.GetAclsForServiceAccount(serviceAccountId);
            var expectedAclCount = 12;
            return acls.Count() == expectedAclCount;
        }

        public async Task<bool> AtLestOneApiKeyExists(string serviceAccountId)
        {
            var apiKeys = await _apiKeyClient.GetApiKeyPairsForServiceAccount(serviceAccountId);
            return apiKeys.Any();
        }
    }
}