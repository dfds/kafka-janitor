using System;
using System.Linq;
using KafkaJanitor.App.Domain.Model;
using Xunit;

namespace KafkaJanitor.Tests.Domain.Model;

public class TestServiceAccountId
{
    [Theory]
    [InlineData("foo")]
    [InlineData("bar")]
    [InlineData("baz")]
    [InlineData("qux")]
    public void returns_expected_string_representation(string expected)
    {
        var sut = ServiceAccountId.Parse(expected);
        Assert.Equal(expected, sut.ToString());
    }

    [Theory]
    [InlineData("foo", "foo")]
    [InlineData("bar", "bar")]
    [InlineData("baz", "baz")]
    [InlineData("qux", "qux")]
    [InlineData("User:foo", "foo")]
    [InlineData("User:bar", "bar")]
    [InlineData("user:foo", "foo")]
    [InlineData("user:bar", "bar")]
    public void parse_returns_expected_when_given_valid_input(string input, string expected)
    {
        var sut = ServiceAccountId.Parse(input);
        Assert.Equal(expected, sut.ToString());
    }

    [Fact]
    public void returns_expected_when_comparing_two_equal_instances()
    {
        var left = ServiceAccountId.Parse("foo");
        var right = ServiceAccountId.Parse("foo");

        Assert.Equal(left, right);
    }

    [Fact]
    public void returns_expected_when_comparing_two_equal_instances_using_operator()
    {
        var left = ServiceAccountId.Parse("foo");
        var right = ServiceAccountId.Parse("foo");

        Assert.True(left == right);
    }

    [Fact]
    public void returns_expected_when_comparing_two_NON_equal_instances()
    {
        var left = ServiceAccountId.Parse("foo");
        var right = ServiceAccountId.Parse("bar");

        Assert.NotEqual(left, right);
    }

    [Fact]
    public void returns_expected_when_comparing_two_NON_equal_instances_using_operator()
    {
        var left = ServiceAccountId.Parse("foo");
        var right = ServiceAccountId.Parse("bar");

        Assert.True(left != right);
    }
}