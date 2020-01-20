using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaJanitor.WebApp.Models
{
    public interface IServiceAccountRepository
    {
        Task<ServiceAccount> Add(ServiceAccount serviceAccount);
        Task Delete(string id);
        
        Task<ServiceAccount> Get(string id);
        Task<IEnumerable<ServiceAccount>> GetAll();
        Task<IEnumerable<ServiceAccount>> GetAllFromCapabilityId(string capabilityId);
        
        Task<bool> Exists(string serviceAccountName);

    }
}