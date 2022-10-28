namespace KafkaJanitor.App.Domain.Model;

public interface IEventSource
{
    string GetEventSourceId();
    IEnumerable<IDomainEvent> GetEvents();
    void ClearEvents();
}