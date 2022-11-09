using System.Text;
using Microsoft.AspNetCore.Http;

namespace KafkaJanitor.Tests.Builders;

public class ServiceAccountResponseBuilder
{
    private string _id;

    public ServiceAccountResponseBuilder()
    {
        _id = "foo";
    }

    public ServiceAccountResponseBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public IResult Build()
    {
        return Results.Content(
            content: $@"{{
                ""id"": ""{_id}""
            }}",
            contentType: "application/json",
            contentEncoding: Encoding.UTF8
        );
    }
}