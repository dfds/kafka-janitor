using Dafda.Outbox;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class OutboxEntryRepository : IOutboxEntryRepository
{
    private readonly KafkaJanitorDbContext _dbContext;

    public OutboxEntryRepository(KafkaJanitorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(IEnumerable<OutboxEntry> outboxEntries)
    {
        await _dbContext.OutboxEntries.AddRangeAsync(outboxEntries);
    }
}