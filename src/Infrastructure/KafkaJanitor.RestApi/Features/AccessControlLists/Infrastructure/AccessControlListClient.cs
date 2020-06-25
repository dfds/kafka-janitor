using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tika.RestClient;
using Tika.RestClient.Features.Acls.Models;

namespace KafkaJanitor.RestApi.Features.AccessControlLists.Infrastructure
{
    public class AccessControlListClient : IAccessControlListClient
    {
        private readonly IRestClient _tikaClient;

        public AccessControlListClient(IRestClient tikaClient)
        {
            _tikaClient = tikaClient;
        }

        public async Task CreateAclsForServiceAccount(string serviceAccountId, string prefix)
        {
            var allAcls = Domain.Models.AccessControlLists.GetAllAcls(serviceAccountId, prefix);
            foreach (var acl in allAcls)
            {
                await _tikaClient.Acls.CreateAsync(acl);
            }
        }

        public async Task DeleteAclsForServiceAccount(string serviceAccountId, string prefix)
        {
            var allAcls = Domain.Models.AccessControlLists.GetAllAcls(serviceAccountId, prefix);

            foreach (var acl in allAcls)
            {
                await _tikaClient.Acls.DeleteAsync(acl);
            }
        }

        public async Task<IEnumerable<Acl>> GetAclsForServiceAccount(string serviceAccountId)
        {
            var serviceAccountIdAsInt = Convert.ToInt64(serviceAccountId);
            var results = await _tikaClient.Acls.GetAllAsync();
            return results.Where(acl => acl.ServiceAccountId == serviceAccountIdAsInt);
        }
    }
}