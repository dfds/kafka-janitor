using System.Collections.Generic;
using System.Threading.Tasks;
using Tika.RestClient.Features.Topics.Models;
using ITopicsClient = Tika.RestClient.Features.Topics.ITopicsClient;

namespace Specifications.TestDoubles
{
    public class TikaTopicsClientSpy : Tika.RestClient.Features.Topics.ITopicsClient
    {
        public List<string> Topics = new List<string>();

        public Task CreateAsync(TopicCreate topicCreate)
        {
            Topics.Add(topicCreate.name);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(string topicName)
        {
            Topics.Remove(topicName);

            return Task.CompletedTask;
        }

        Task<IEnumerable<string>> ITopicsClient.GetAllAsync()
        {
            return Task.FromResult((IEnumerable<string>) Topics);
        }
    }
}