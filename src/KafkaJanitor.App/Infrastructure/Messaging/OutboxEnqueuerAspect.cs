using Aspectly;
using Dafda.Outbox;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.App.Infrastructure.Persistence;

namespace KafkaJanitor.App.Infrastructure.Messaging;

public class OutboxEnqueuerAspect : IAspect
{
    private readonly ILogger<OutboxEnqueuerAspect> _logger;
    private readonly KafkaJanitorDbContext _dbContext;
    private readonly OutboxQueue _outbox;

    public OutboxEnqueuerAspect(ILogger<OutboxEnqueuerAspect> logger, KafkaJanitorDbContext dbContext, OutboxQueue outbox)
    {
        _logger = logger;
        _dbContext = dbContext;
        _outbox = outbox;
    }

    public async Task Invoke(AspectContext context, AspectDelegate next)
    {
        await next();
        await EnqueueDomainEvents();
    }

    private async Task EnqueueDomainEvents()
    {
        var aggregateDomainEvents = GetAggregates();

        if (aggregateDomainEvents.Length > 1)
        {
            // TODO [jandr@2022-11-10]: rethink this - maybe remove
            throw new InvalidOperationException("Multiple aggregates modified in the same transaction");
        }

        foreach (var aggregate in aggregateDomainEvents)
        {
            var domainEvents = aggregate
                .GetEvents()
                .ToArray();

            await _outbox.Enqueue(domainEvents);
            aggregate.ClearEvents();

            foreach (var domainEvent in domainEvents)
            {
                _logger.LogTrace("Enqueued domain event {DomainEvent} in the outbox for aggregate {Aggregate}", domainEvent.GetType().Name, aggregate.GetType().Name);
            }
        }
    }

    private IEventSource[] GetAggregates()
    {
        return _dbContext
            .ChangeTracker
            .Entries<IEventSource>()
            .Where(x => x.Entity.GetEvents().Any())
            .Select(x => x.Entity)
            .ToArray();
    }
}