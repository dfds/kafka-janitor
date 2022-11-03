using Moq;

namespace KafkaJanitor.Tests.TestDoubles;

public static class Dummy
{
    public static T Of<T>() where T : class => new Mock<T>().Object;
}