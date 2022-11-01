using KafkaJanitor.App.Domain.Exceptions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Infrastructure.Persistence;

public class TopicProvisioningProcessRepository : ITopicProvisioningProcessRepository
{
    private readonly KafkaJanitorDbContext _dbContext;

    public TopicProvisioningProcessRepository(KafkaJanitorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(TopicProvisioningProcess process)
    {
        await _dbContext.TopicProvisioningProcesses.AddAsync(process);
    }

    public async Task<TopicProvisioningProcess> Get(TopicProvisionProcessId id)
    {
        var result = await _dbContext.TopicProvisioningProcesses.FindAsync(id);

        if (result is null)
        {
            throw new DoesNotExistException($"A topic provisioning process with id \"{id}\" does not exist.");
        }

        return result;
    }

    public async Task<IEnumerable<TopicProvisioningProcess>> FindAllActiveBy(CapabilityRootId capabilityRootId)
    {
        return await _dbContext.TopicProvisioningProcesses
            .Where(x => x.CapabilityRootId == capabilityRootId)
            .Where(x => x.IsCompleted == false)
            .ToListAsync();
    }
}