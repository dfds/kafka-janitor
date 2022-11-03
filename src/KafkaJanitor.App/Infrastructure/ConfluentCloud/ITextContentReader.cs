namespace KafkaJanitor.App.Infrastructure.ConfluentCloud;

public interface ITextContentReader
{
    Task<string[]> ReadAllLines();
}