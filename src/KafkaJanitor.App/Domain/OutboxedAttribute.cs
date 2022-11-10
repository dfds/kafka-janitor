namespace KafkaJanitor.App.Domain;

[AttributeUsage(AttributeTargets.Method)]
public class OutboxedAttribute : Attribute
{

}