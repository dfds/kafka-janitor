using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.WebApp.Controllers
{
    [Route("api/topics")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;

        public TopicController(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var topics = await _topicRepository.GetAll();

            var result = new
            {
                Items = topics.OrderBy(x => x.Name).Select(x => new
                {
                    Name = x.Name,
                    Partitions = x.Partitions
                })
            };
            
            return Ok(result);
        }
        
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] TopicInput input)
        {
            if (await _topicRepository.Exists(input.Name))
            {
                return Conflict(new
                {
                    Message = "Topic already exists."
                });
            }

            var topic = new Topic(input.Name);
            await _topicRepository.Add(topic);

            return Ok(topic);
        }
    }

    public class TopicInput
    {
        public string Name { get; set; }
    }
}