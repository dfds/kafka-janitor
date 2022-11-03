using System.ComponentModel.DataAnnotations;

namespace KafkaJanitor.App.Infrastructure.Api;

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