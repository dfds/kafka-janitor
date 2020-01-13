using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Infrastructure.Tika.Model;

namespace KafkaJanitor.WebApp.Infrastructure.Tika
{
    public class TikaClient : ITikaClient
    {
        public async Task GetApiKeys()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateApiKey(object payload)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteApiKey(object payload)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<ServiceAccount>> GetServiceAccounts()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ServiceAccount> CreateServiceAccount()
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteServiceAccount(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task GetTopics()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateTopic()
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteTopic()
        {
            throw new System.NotImplementedException();
        }

        public async Task GetAcls()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateAcl()
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAcl()
        {
            throw new System.NotImplementedException();
        }
    }
}