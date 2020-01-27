using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;

namespace KafkaJanitor.RestClient.Features.Topics
{
    public interface ITopicsClient
    {
        Task CreateAsync(Topic input);

        Task<IEnumerable<Topic>> GetAllAsync();
    }
}