using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tika.RestClient.Features.Topics.Models;
using ITopicsClient = Tika.RestClient.Features.Topics.ITopicsClient;

namespace Specifications.TestDoubles
{
    public class TikaTopicsClientSpy : ITopicsClient
    {
        public readonly List<TopicDescription> TopicDescriptions = new List<TopicDescription>();
        public Task<TopicDescription> DescribeAsync(string topicName, string clusterId)
        {
            var topicDescription = TopicDescriptions
                .Single(t => 
                    t.name == topicName
                );

            
            return Task.FromResult(topicDescription);
        }

        public Task CreateAsync(TopicCreate topicCreate, string clusterId)
        {
            var topicDescription = new TopicDescription
            {
                name = topicCreate.Name,
                partitionCount = topicCreate.PartitionCount,
                configurations = topicCreate.Configurations
            };
            
            TopicDescriptions.Add(topicDescription);
            
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string topicName, string clusterId)
        {
            var topicDescription = TopicDescriptions
                .Single(t => 
                    t.name == topicName
                );

            TopicDescriptions.Remove(topicDescription);
            
            return Task.CompletedTask;
        }

        Task<IEnumerable<string>> ITopicsClient.GetAllAsync(string clusterId)
        {
            return Task.FromResult(TopicDescriptions.Select(t => t.name));
        }
    }
}