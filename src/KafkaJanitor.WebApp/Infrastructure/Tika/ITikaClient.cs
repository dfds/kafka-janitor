using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Infrastructure.Tika.Model;

namespace KafkaJanitor.WebApp.Infrastructure.Tika
{
    public interface ITikaClient
    {
        // Api keys
        Task GetApiKeys();
        Task CreateApiKey(object payload);
        Task DeleteApiKey(object payload);
        
        // Service accounts
        Task<IEnumerable<ServiceAccount>> GetServiceAccounts();
        Task<ServiceAccount> CreateServiceAccount(string name, string description = null);
        Task DeleteServiceAccount(string id);
        
        // Topics
        Task GetTopics();
        Task CreateTopic();
        Task DeleteTopic();
        
        // ACL
        Task GetAcls();
        Task CreateAcl();
        Task DeleteAcl();
    }
}