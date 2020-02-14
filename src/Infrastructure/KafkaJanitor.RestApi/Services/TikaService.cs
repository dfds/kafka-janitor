using System;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Tika.RestClient;
using Tika.RestClient.Features.Acls.Models;
using Tika.RestClient.Features.ServiceAccounts.Models;

namespace KafkaJanitor.RestApi.Services
{
    public class TikaService : ITikaService
    {
        private readonly IRestClient _tikaClient;
        
        public TikaService(IRestClient tikaClient)
        {
            _tikaClient = tikaClient;
        }
        
        public async Task CreateTopic()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ServiceAccount> CreateServiceAccount(Capability capability)
        {
            return await _tikaClient.ServiceAccounts.CreateAsync(new ServiceAccountCreate {
                name = $"{capability.Name}_sa",
                description = "Creating with TikaService using KafkaJanitor"
            });
        }

        public async Task CreateAcl(Capability capability, ServiceAccount serviceAccount)
        {
            var serviceAccountId = Convert.ToInt64(serviceAccount.Id);
            // Topic
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "WRITE", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "CREATE", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "READ", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "DESCRIBE", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "DESCRIBE-CONFIGS", capability.Name));

            // ConsumerGroup
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "WRITE", "", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "CREATE", "", capability.Name));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, true, "READ", "", capability.Name));
            
            // DENY
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, false, "alter"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, false, "alter-configs"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, false, "cluster-action"));
            await _tikaClient.Acls.CreateAsync(new AclCreateDelete(serviceAccountId, false, "create", "'*'"));
        }
    }
}