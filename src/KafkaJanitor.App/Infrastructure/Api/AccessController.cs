using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.App.Infrastructure.Api;

[ApiController]
[Route("api/access")]
public class AccessController : ControllerBase
{
    [HttpPost("request")]
    public IActionResult RequestAccess([FromBody] AccessRequest input)
    {
        return Ok();
    }

    public class AccessRequest
    {
        
    }
}