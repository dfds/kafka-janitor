using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;

namespace KafkaJanitor.RestApi.Features.Topics.Domain
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetAll();

        Task<Topic> DescribeAsync(string topicName);
        Task Add(Topic topic);

        Task<bool> Exists(string topicName);
    }
}