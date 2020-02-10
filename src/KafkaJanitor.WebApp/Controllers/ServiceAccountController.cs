using System;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Infrastructure.Services;
using KafkaJanitor.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.WebApp.Controllers
{
    [Route("api/serviceaccount")]
    public class ServiceAccountController : ControllerBase
    {
        private readonly ITikaService _tikaService;
        
        public ServiceAccountController(ITikaService tikaService)
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