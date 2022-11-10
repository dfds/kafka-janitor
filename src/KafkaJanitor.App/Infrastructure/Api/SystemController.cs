using System.Text;
using KafkaJanitor.App.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Api;

[Route("system")]
[ApiController]
public class SystemController : ControllerBase
{
    private readonly KafkaJanitorDbContext _dbContext;

    public SystemController(KafkaJanitorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("ping")]
    public IActionResult PingPong() => Content("pong");

    [HttpGet("dbschema")]
    public IActionResult DbSchema()
    {
        var script = _dbContext.Database.GenerateCreateScript();
        return Content(script, "application/sql", Encoding.UTF8);
    }
}