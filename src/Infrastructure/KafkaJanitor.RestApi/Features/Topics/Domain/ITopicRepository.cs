using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;

namespace KafkaJanitor.RestApi.Features.Topics.Domain
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetAll(string clusterId = null);

        Task<Topic> DescribeAsync(string topicName, string clusterId = null);
        Task Add(Topic topic, string clusterId = null);

        Task<bool> Exists(string topicName, string clusterId = null);
    }
}