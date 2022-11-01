namespace KafkaJanitor.App.Domain.Exceptions;

public class DoesNotExistException : Exception
{
    public DoesNotExistException(string message) : base(message)
    {
        
    }
}