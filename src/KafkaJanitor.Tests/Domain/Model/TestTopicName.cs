using System;
using KafkaJanitor.App.Domain.Model;
using KafkaJanitor.Tests.Builders;
using Xunit;

namespace KafkaJanitor.Tests.Domain.Model;

public class TestTopicName
{
    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public void returns_expected_capability_root_id_when_initialized(string value)
    {
        var sut = new TopicNameBuilder()
            .WithCapabilityRootId(value)
            .Build();

        Assert.Equal(value, sut.CapabilityRootId);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public void returns_expected_name_when_initialized(string value)
    {
        var sut = new TopicNameBuilder()
            .WithName(value)
            .Build();

        Assert.Equal(value, sut.Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void returns_expected_is_public_when_initialized(bool value)
    {
        var sut = new TopicNameBuilder()
            .WithIsPublic(value)
            .Build();

        Assert.Equal(value, sut.IsPublic);
    }

    [Theory]
    [InlineData("foo", "bar")]
    [InlineData("baz", "qux")]
    public void returns_expected_string_representation_when_public(string rootId, string name)
    {
        var sut = new TopicNameBuilder()
            .WithCapabilityRootId(rootId)
            .WithName(name)
            .WithIsPublic(true)
            .Build();

        Assert.Equal(
            expected: $"pub.{rootId}.{name}",
            actual: sut.ToString()
        );
    }

    [Theory]
    [InlineData("foo", "bar")]
    [InlineData("baz", "qux")]
    public void returns_expected_string_representation_when_NOT_public(string rootId, string name)
    {
        var sut = new TopicNameBuilder()
            .WithCapabilityRootId(rootId)
            .WithName(name)
            .WithIsPublic(false)
            .Build();

        Assert.Equal(
            expected: $"{rootId}.{name}",
            actual: sut.ToString()
        );
    }

    [Theory]
    [InlineData("FOO", "bar")]
    [InlineData("fOo", "bAr")]
    public void string_representation_is_in_expected_casing(string rootId, string name)
    {
        var sut = new TopicNameBuilder()
            .WithCapabilityRootId(rootId)
            .WithName(name)
            .WithIsPublic(false)
            .Build();

        Assert.Equal(
            expected: $"{rootId}.{name}".ToLowerInvariant(),
            actual: sut.ToString()
        );
    }

    [Fact]
    public void parse_valid_topic_name_returns_expected_when_parsing_valid_public_topic()
    {
        var sut = TopicName.Parse("pub.foo.bar");

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.True(sut.IsPublic);
    }

    [Fact]
    public void parse_valid_topic_name_ignores_casing_when_parsing_valid_public_topic()
    {
        var sut = TopicName.Parse("PUB.FOO.BAR");

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.True(sut.IsPublic);
    }

    [Fact]
    public void parse_valid_topic_name_returns_expected_when_parsing_valid_NON_public_topic()
    {
        var sut = TopicName.Parse("foo.bar");

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.False(sut.IsPublic);
    }

    [Fact]
    public void parse_valid_topic_name_ignores_casing_when_parsing_valid_NON_public_topic()
    {
        var sut = TopicName.Parse("foo.bar");

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.False(sut.IsPublic);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("      ")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("...")]
    public void parse_throws_expected_exception_when_parsing_invalid_input(string? invalidInput)
    {
        Assert.Throws<FormatException>(() => TopicName.Parse(invalidInput!));
    }

    [Fact]
    public void try_parse_returns_expected_result_when_parsing_valid_input()
    {
        var result = TopicName.TryParse("foo.bar", out _);
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("      ")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("...")]
    public void try_parse_returns_expected_result_when_parsing_invalid_input(string? invalidInput)
    {
        var result = TopicName.TryParse(invalidInput!, out _);
        Assert.False(result);
    }

    [Fact]
    public void try_parse_valid_topic_name_returns_expected_instance_when_parsing_valid_public_topic()
    {
        TopicName.TryParse("pub.foo.bar", out var sut);

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.True(sut.IsPublic);
    }

    [Fact]
    public void try_parse_valid_topic_name_ignores_casing_and_returns_expected_instance_when_parsing_valid_public_topic()
    {
        TopicName.TryParse("PUB.FOO.BAR", out var sut);

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.True(sut.IsPublic);
    }

    [Fact]
    public void try_parse_valid_topic_name_returns_expected_instance_when_parsing_valid_NON_public_topic()
    {
        TopicName.TryParse("foo.bar", out var sut);

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.False(sut.IsPublic);
    }

    [Fact]
    public void try_parse_valid_topic_name_ignores_casing_and_returns_expected_instance_when_parsing_valid_NON_public_topic()
    {
        TopicName.TryParse("foo.bar", out var sut);

        Assert.Equal("foo", sut.CapabilityRootId);
        Assert.Equal("bar", sut.Name);
        Assert.False(sut.IsPublic);
    }

    [Fact]
    public void returns_expected_when_comparing_two_equal_instances()
    {
        var left = new TopicNameBuilder()
            .WithCapabilityRootId("foo")
            .WithName("bar")
            .WithIsPublic(true)
            .Build();
        
        var right = new TopicNameBuilder()
            .WithCapabilityRootId("foo")
            .WithName("bar")
            .WithIsPublic(true)
            .Build();

        Assert.Equal(left, right);
    }

    [Fact]
    public void returns_expected_when_comparing_two_equal_instances_using_operator()
    {
        var left = new TopicNameBuilder()
            .WithCapabilityRootId("foo")
            .WithName("bar")
            .WithIsPublic(true)
            .Build();
        
        var right = new TopicNameBuilder()
            .WithCapabilityRootId("foo")
            .WithName("bar")
            .WithIsPublic(true)
            .Build();

        Assert.True(left == right);
    }

    [Fact]
    public void returns_expected_when_comparing_two_NON_equal_instances()
    {
        var left = new TopicNameBuilder()
            .WithCapabilityRootId("foo")
            .WithName("bar")
            .WithIsPublic(true)
            .Build();
        
        var right = new TopicNameBuilder()
            .WithCapabilityRootId("baz")
            .WithName("qux")
            .WithIsPublic(false)
            .Build();

        Assert.NotEqual(left, right);
    }

    [Fact]
    public void returns_expected_when_comparing_two_NON_equal_instances_using_operator()
    {
        var left = new TopicNameBuilder()
            .WithCapabilityRootId("foo")
            .WithName("bar")
            .WithIsPublic(true)
            .Build();
        
        var right = new TopicNameBuilder()
            .WithCapabilityRootId("baz")
            .WithName("qux")
            .WithIsPublic(false)
            .Build();

        Assert.True(left != right);
    }
}