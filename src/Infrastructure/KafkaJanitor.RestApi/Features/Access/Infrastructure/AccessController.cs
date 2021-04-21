using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Access.Application;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.RestApi.Features.Access.Infrastructure
{
    [Route(Routes.ACCESS_ROUTE)]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _accessService;

        public AccessController(IAccessService accessService)
        {
            _accessService = accessService;
        }
        
        [HttpPost("request")]
        public async Task<IActionResult> RequestServiceAccount([FromBody] ServiceAccountRequestInput input)
        {
            var capability = new Capability
            {
                Id = input.CapabilityId,
                Name = input.CapabilityName,
                RootId = input.CapabilityRootId
            };
            
            await _accessService.ProvideAccess(
                capability, 
                input.TopicPrefix,
                input.ClusterId
            );
         
            return Ok();
        }

    }
    
    public class ServiceAccountRequestInput
    {
        public string CapabilityName { get; set; }
        public string CapabilityId { get; set; }
        public string CapabilityRootId { get; set; }
        public string TopicPrefix { get; set; }
        public string ClusterId { get; set; }
    }
}