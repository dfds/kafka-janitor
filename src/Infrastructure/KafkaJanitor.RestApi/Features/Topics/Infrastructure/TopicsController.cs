using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.RestApi.Features.Topics.Infrastructure
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

        [HttpGet("{topicName}")]
        public async Task<IActionResult> DescribeAsync([FromRoute] string topicName)
        {
            var topics = await _topicRepository.DescribeAsync(topicName);

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