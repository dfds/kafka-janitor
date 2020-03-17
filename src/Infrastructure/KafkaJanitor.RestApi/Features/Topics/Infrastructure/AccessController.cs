using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.AccessControlLists.Infrastructure;
using KafkaJanitor.RestApi.Features.ApiKeys;
using KafkaJanitor.RestApi.Features.ServiceAccounts.Infrastructure;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault;
using KafkaJanitor.RestApi.Features.Vault.Model;
using Microsoft.AspNetCore.Mvc;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Features.Topics.Infrastructure
{
    [Route(Routes.ACCESS_ROUTE)]
    public class AccessController : ControllerBase
    {
        private readonly IAccessControlListClient _accessControlListService;
        private readonly IServiceAccountClient _serviceAccountClient;
        private readonly IApiKeyClient _apiKeyClient;
        private readonly IVault _vault;
        public AccessController(
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

        [HttpPost("request")]
        public async Task<IActionResult> RequestServiceAccount([FromBody] ServiceAccountRequestInput input)
        {
            var cap = new Capability
            {
                Id = input.CapabilityId,
                Name = input.CapabilityName,
                RootId = input.CapabilityRootId
            };

            ServiceAccount serviceAccount = null;

            var doesServiceaccountExist = new Func<Task<bool>>(async () =>
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
            
            var isExpectedAmountOfAclsInPlace = new Func<string,Task<bool>>(async (string serviceAccountId) =>
            {
                var acls = await _accessControlListService.GetAclsForServiceAccount(serviceAccountId);
                return acls.Count() == 12;
            });
            
            var isExpectedAmountOfApiKeysInPlace = new Func<string,Task<bool>>(async (string serviceAccountId) =>
            {
                var apiKeys = await _apiKeyClient.GetApiKeyPairsForServiceAccount(serviceAccountId);
                return apiKeys.Any();
            });
            
            if (!await doesServiceaccountExist())
            {
                serviceAccount = await _serviceAccountClient.CreateServiceAccount(cap);
            }
            else
            {
                serviceAccount = await _serviceAccountClient.GetServiceAccount(cap);
            }

            if (!await isExpectedAmountOfAclsInPlace(serviceAccount.Id))
            {
                await _accessControlListService.CreateAclsForServiceAccount(serviceAccount.Id, input.TopicPrefix);
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

            return Ok();
        }

    }
    
    public class ServiceAccountRequestInput
    {
        public string CapabilityName { get; set; }
        public string CapabilityId { get; set; }
        public string CapabilityRootId { get; set; }
        public string TopicPrefix { get; set; }
    }
}