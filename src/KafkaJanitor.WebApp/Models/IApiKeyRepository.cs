using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaJanitor.WebApp.Models
{
    public interface IApiKeyRepository
    {
        Task<ApiKey> Add(ApiKey apiKey);
        Task Delete(string id);
        
        Task<ApiKey> Get(string id);
        Task<IEnumerable<ApiKey>> GetAll();
        Task<IEnumerable<ApiKey>> GetAllFromServiceAccountId(string serviceAccountId);
        
        Task<bool> Exists(string apiKey);
    }
}