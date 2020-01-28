using System.Threading.Tasks;
using KafkaJanitor.RestApi.FakeTikaRestClient;
using KafkaJanitor.RestApi.Features.Topics.Models;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.RestApi.Features.Topics
{
    [Route(Routes.TOPICS_ROUTE)]
    public class TopicsController : ControllerBase
    {
        private readonly ITikaRestClient _tikaRestClient;

        public TopicsController(ITikaRestClient tikaRestClient)
        {
            _tikaRestClient = tikaRestClient;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAllAsync()
        {
            var topics = await _tikaRestClient.GetAllAsync();

            return Ok(topics);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromBody] Topic input)
        {
            if (await _tikaRestClient.ExistsAsync(input.Name))
            {
                return Conflict(new
                {
                    Message = "Topic already exists."
                });
            }

            var topic = await _tikaRestClient.AddAsync(input.Name);

            return Ok(topic);
        }
    }
}