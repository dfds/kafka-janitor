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
            Capability cap,
            string topicPrefix
        )
        {
            ServiceAccount serviceAccount = null;

            Func<Task<bool>> doesServiceAccountExist = new Func<Task<bool>>(async () =>
            {
                try
                {
                    var sa = await _serviceAccountClient.GetServiceAccount(cap);
                }
                catch (InvalidOperationException ex)
                {
                    return false;
                }

                return true;
            });

            Func<string, Task<bool>> isExpectedAmountOfAclsInPlace = new Func<string, Task<bool>>(
                async (string serviceAccountId) =>
                {
                    var acls = await _accessControlListService.GetAclsForServiceAccount(serviceAccountId);
                    return acls.Count() == 12;
                });

            Func<string, Task<bool>> isExpectedAmountOfApiKeysInPlace = new Func<string, Task<bool>>(
                async (string serviceAccountId) =>
                {
                    var apiKeys = await _apiKeyClient.GetApiKeyPairsForServiceAccount(serviceAccountId);
                    return apiKeys.Any();
                });

            if (!await doesServiceAccountExist())
            {
                serviceAccount = await _serviceAccountClient.CreateServiceAccount(cap);
            }
            else
            {
                serviceAccount = await _serviceAccountClient.GetServiceAccount(cap);
            }

            if (!await isExpectedAmountOfAclsInPlace(serviceAccount.Id))
            {
                await _accessControlListService.CreateAclsForServiceAccount(serviceAccount.Id, topicPrefix);
            }

            if (!await isExpectedAmountOfApiKeysInPlace(serviceAccount.Id))
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
        }
    }
}