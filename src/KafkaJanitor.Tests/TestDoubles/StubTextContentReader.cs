using System;
using System.Threading.Tasks;
using KafkaJanitor.App.Infrastructure.ConfluentCloud;

namespace KafkaJanitor.Tests.TestDoubles;

public class StubTextContentReader : ITextContentReader
{
    private readonly string[] _result;

    private StubTextContentReader(string[] result)
    {
        _result = result;
    }

    public Task<string[]> ReadAllLines() => Task.FromResult(_result);

    public static StubTextContentReader AsEmpty() => new StubTextContentReader(Array.Empty<string>());
    public static StubTextContentReader Containing(params string[] lines) => new StubTextContentReader(lines);
}