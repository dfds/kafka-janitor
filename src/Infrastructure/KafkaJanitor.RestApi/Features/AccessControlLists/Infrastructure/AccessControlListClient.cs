using System;
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


        public async Task CreateAclsForServiceAccount(string serviceAccountId, string capabilityName)
        {
            var serviceAccountIdAsInt = Convert.ToInt64(serviceAccountId);
            // Topic
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", capabilityName));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", capabilityName));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "READ", capabilityName));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "DESCRIBE", capabilityName));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "DESCRIBE-CONFIGS", capabilityName));

            // ConsumerGroup
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", "", capabilityName));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", "", capabilityName));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, true, "READ", "", capabilityName));
            
            // DENY
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "alter"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "alter-configs"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "cluster-action"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountIdAsInt, false, "create", "'*'"));
        }
    }
}