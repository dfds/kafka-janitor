using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class TopicNameConverter : ValueConverter<TopicName, string>
{
    public TopicNameConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<TopicName, string>> ToDatabaseType => value => value.ToString();
    private static Expression<Func<string, TopicName>> FromDatabaseType => value => TopicName.Parse(value);
}