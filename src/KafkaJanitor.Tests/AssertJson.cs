using System.IO;
using System.Text.Json;
using Xunit;

namespace KafkaJanitor.Tests;

public static class AssertJson
{
    public static void Equal(string expected, string actual)
    {
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(actual))
        {
            Assert.Equal(expected, actual);
        }

        Equal(
            expected: JsonDocument.Parse(expected),
            actual: JsonDocument.Parse(actual)
        );
    }

    public static void Equal(JsonDocument expected, JsonDocument actual) => Assert.Equal(
        expected: ToString(expected),
        actual: ToString(actual)
    );

    private static string ToString(JsonDocument document)
    {
        using var stream = new MemoryStream();

        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });
        document.WriteTo(writer);
        writer.Flush();
        stream.Position = 0;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}