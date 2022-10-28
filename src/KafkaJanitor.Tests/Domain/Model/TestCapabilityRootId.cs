using KafkaJanitor.App.Domain.Model;
using Xunit;

namespace KafkaJanitor.Tests.Domain.Model;

public class TestCapabilityRootId
{
    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public void parse_returns_expected_when_parsing_valid_input(string input)
    {
        var sut = CapabilityRootId.Parse(input);
        Assert.Equal(input, sut.ToString());
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public void try_parse_returns_expected_result_when_parsing_valid_input(string input)
    {
        var result = CapabilityRootId.TryParse(input, out _);
        Assert.True(result);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public void try_parse_returns_expected_instance_when_parsing_valid_input(string input)
    {
        CapabilityRootId.TryParse(input, out var result);
        Assert.Equal(input, result.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("        ")]
    public void try_parse_returns_expected_result_when_parsing_invalid_input(string? input)
    {
        var result = CapabilityRootId.TryParse(input, out _);
        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("        ")]
    public void try_parse_returns_expected_instance_when_parsing_invalid_input(string? input)
    {
        CapabilityRootId.TryParse(input, out var result);
        Assert.Null(result);
    }
}