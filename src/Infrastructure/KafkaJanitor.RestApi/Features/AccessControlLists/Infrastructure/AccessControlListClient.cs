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
            var serviceAccountIdAsInt = Convert.ToInt64(serviceAccountId);
            // Topic
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", prefix));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", prefix));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "READ", prefix));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "DESCRIBE", prefix));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "DESCRIBE-CONFIGS", prefix));

            // ConsumerGroup
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", "", prefix));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", "", prefix));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "READ", "", prefix));
            
            // DENY
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "alter"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "alter-configs"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "cluster-action"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "create", "'*'"));
        }

        public async Task<IEnumerable<Acl>> GetAclsForServiceAccount(string serviceAccountId)
        {
            var serviceAccountIdAsInt = Convert.ToInt64(serviceAccountId);
            var results = await _tikaClient.Acls.GetAllAsync();
            return results.Where(acl => acl.ServiceAccountId == serviceAccountIdAsInt);
        }
    }
}