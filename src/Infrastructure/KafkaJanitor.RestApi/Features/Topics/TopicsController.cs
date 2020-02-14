using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.RestApi.Features.Topics
{
    [Route(Routes.TOPICS_ROUTE)]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;


        public TopicsController(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAllAsync()
        {
            var topics = await _topicRepository.GetAll();

            return Ok(topics);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromBody] Topic input)
        {
            if (await _topicRepository.Exists(input.Name))
            {
                return Conflict(new
                {
                    Message = "Topic already exists."
                });
            }

            await _topicRepository.Add(input);

            return Ok(input);
        }
    }
}