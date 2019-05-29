using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaJanitor.WebApp.Models
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetAll();
        Task Add(Topic topic);

        Task<bool> Exists(string topicName);
    }
}