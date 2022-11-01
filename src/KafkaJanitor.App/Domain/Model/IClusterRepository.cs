namespace KafkaJanitor.App.Domain.Model;

public interface IClusterRepository
{
    Task<IEnumerable<Cluster>> GetAll();
}