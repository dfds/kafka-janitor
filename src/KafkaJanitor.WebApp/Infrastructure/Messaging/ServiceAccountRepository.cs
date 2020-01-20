using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;
using Tika.Client;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class ServiceAccountRepository : IServiceAccountRepository
    {
        private readonly ITikaClient _tikaClient;
        
        public ServiceAccountRepository(ITikaClient tikaClient)
        {
            _tikaClient = tikaClient;
        }
        
        public async Task<ServiceAccount> Add(ServiceAccount serviceAccount)
        {
            var resp = await _tikaClient.CreateServiceAccount(name: serviceAccount.Name, description: serviceAccount.Description);
            return new ServiceAccount
            {
                Id = resp.Id,
                Name = resp.Name,
                Description = resp.Description
            };
        }

        public async Task Delete(string id)
        {
            await _tikaClient.DeleteServiceAccount(id);
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

        public async Task<IEnumerable<ServiceAccount>> GetAllFromCapabilityId(string capabilityId)
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