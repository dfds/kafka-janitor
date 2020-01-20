using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;
using Tika.Client;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class ApiKeyRepository
    {
        private readonly ITikaClient _tikaClient;
        
        public ApiKeyRepository(ITikaClient tikaClient)
        {
            _tikaClient = tikaClient;
        }
        
        public async Task<ApiKey> Add(string description, string serviceAccountId)
        {
            var resp = await _tikaClient.CreateApiKey(serviceAccountId, description);
            return new ApiKey
            {
                Id = "",
                Key = resp.Key,
                Secret = resp.Secret,
                ServiceAccountId = serviceAccountId
            };
        }

        public async Task Delete(string id)
        {
            await _tikaClient.DeleteApiKey(id);
        }

        public async Task<ServiceAccount> Get(string id)
        {
            var resp = await _tikaClient.GetServiceAccounts();
            var respFirst = resp.First(sa => sa.Id.Equals(id));

            return new ServiceAccount
            {
                Id = respFirst.Id,
                Name = respFirst.Name,
                Description = respFirst.Description
            };
        }

        public async Task<IEnumerable<ServiceAccount>> GetAll()
        {
            var resp = await _tikaClient.GetServiceAccounts();
            return resp.Select(sa => new ServiceAccount
            {
                Name = sa.Name,
                Id = sa.Id,
                Description = sa.Description
            });
        }

        public async Task<IEnumerable<ServiceAccount>> GetAllFromServiceAccountId(string serviceAccountId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Exists(string serviceAccountName)
        {
            var resp = await _tikaClient.GetServiceAccounts();
            try
            {
                var respFirst = resp.First(sa => sa.Name.Equals(serviceAccountName));
                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }        
    }
}