using System.ComponentModel.DataAnnotations;
using KafkaJanitor.App.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.App.Infrastructure.Api;

[ApiController]
[Route("api/topics")]
public class TopicController : ControllerBase
{
    private readonly IConfluentGateway _confluentGateway;

    public TopicController(IConfluentGateway confluentGateway)
    {
        _confluentGateway = confluentGateway;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] string? clusterId)
    {
        // TODO [jandr@2022-10-12]: don't take a required param as a optional query param!! (in support of legacy api)

        if (!ClusterId.TryParse(clusterId, out var cluster))
        {
            ModelState.AddModelError(nameof(clusterId), "Missing required cluster id as query parameter.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var topics = await _confluentGateway.GetAllBy(cluster);

        var dto = topics
            .Select(x => new {Name = x.Name})
            .ToArray();

        return Ok(dto);
    }

    [HttpPost("")]
    public async Task<IActionResult> ProvisionNewTopic([FromBody] ProvisionNewTopicRequest input)
    {
        var exists = await _confluentGateway.Exists(input.Name!);
        if (exists)
        {
            return Conflict();
        }

        return Ok();
    }
}

public class ProvisionNewTopicRequest
{
    [Required]
    public string? ClusterId { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public int? Partitions { get; set; }

    [Required]
    public int? Retention { get; set; }
}