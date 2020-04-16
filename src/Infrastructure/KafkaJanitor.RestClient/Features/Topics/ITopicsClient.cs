using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestClient.Features.Topics.Models;

namespace KafkaJanitor.RestClient.Features.Topics
{
    public interface ITopicsClient
    {
        Task CreateAsync(Topic input);

        Task<IEnumerable<Topic>> GetAllAsync();

        Task<Topic> DescribeAsync(string topicName);
    }
}