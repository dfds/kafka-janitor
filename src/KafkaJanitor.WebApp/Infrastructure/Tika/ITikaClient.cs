using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Infrastructure.Tika.Model;

namespace KafkaJanitor.WebApp.Infrastructure.Tika
{
    public interface ITikaClient
    {
        // Api keys
        Task<IEnumerable<ApiKey>> GetApiKeys();
        Task<ApiKey> CreateApiKey(string serviceAccountId, string description = "");
        Task DeleteApiKey(string key);
        
        // Service accounts
        Task<IEnumerable<ServiceAccount>> GetServiceAccounts();
        Task<ServiceAccount> CreateServiceAccount(string name, string description = null);
        Task DeleteServiceAccount(string id);
        
        // Topics
        Task GetTopics();
        Task CreateTopic();
        Task DeleteTopic();
        
        // ACL
        Task<IEnumerable<Acl>> GetAcls();
        Task CreateAcl(string serviceAccountId, bool allow, string operation, string topicPrefix, string consumerGroupPrefix);
        Task DeleteAcl(string serviceAccountId, bool allow, string operation, string topicPrefix, string consumerGroupPrefix);
    }
}