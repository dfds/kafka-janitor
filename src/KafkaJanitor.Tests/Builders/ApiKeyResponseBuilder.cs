using System.Text;
using Microsoft.AspNetCore.Http;

namespace KafkaJanitor.Tests.Builders;

public class ApiKeyResponseBuilder
{
    private string _id;
    private string _secret;
    private string _resourceId;

    public ApiKeyResponseBuilder()
    {
        _id = "foo";
        _secret = "bar";
        _resourceId = "baz";
    }

    public ApiKeyResponseBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ApiKeyResponseBuilder WithSecret(string secret)
    {
        _secret = secret;
        return this;
    }

    public ApiKeyResponseBuilder WithResourceId(string resourceId)
    {
        _resourceId = resourceId;
        return this;
    }

    public IResult Build()
    {
        return Results.Content(
            content: $@"{{
                ""id"": ""{_id}"",
                ""spec"": {{
                    ""resource"": {{
                        ""id"": ""{_resourceId}""
                    }},
                    ""secret"": ""{_secret}""
                }}
            }}",
            contentType: "application/json",
            contentEncoding: Encoding.UTF8
        );
    }
}