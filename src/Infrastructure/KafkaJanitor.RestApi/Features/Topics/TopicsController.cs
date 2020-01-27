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

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] TopicCreate input)
        {
            if (await _tikaRestClient.Exists(input.Name))
            {
                return Conflict(new
                {
                    Message = "Topic already exists."
                });
            }

            var topic = await _tikaRestClient.Add(input.Name);

            return Ok(topic);
        }
    }
}