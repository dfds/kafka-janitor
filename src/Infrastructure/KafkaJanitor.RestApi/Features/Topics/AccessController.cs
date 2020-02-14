using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Models;
using KafkaJanitor.RestApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.RestApi.Features.Topics
{
    [Route(Routes.ACCESS_ROUTE)]
    public class AccessController : ControllerBase
    {
        private readonly ITikaService _tikaService;
        
        public AccessController(ITikaService tikaService)
        {
            _tikaService = tikaService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestServiceAccount([FromBody] ServiceAccountRequestInput input)
        {
            var cap = new Capability
            {
                Id = input.CapabilityId,
                Name = input.CapabilityName
            };
            
            var serviceAccount = await _tikaService.CreateServiceAccount(cap);
            await _tikaService.CreateAcl(cap, serviceAccount);

            return Ok();
        }

    }
    
    public class ServiceAccountRequestInput
    {
        public string CapabilityName { get; set; }
        public string CapabilityId { get; set; }
    }
}