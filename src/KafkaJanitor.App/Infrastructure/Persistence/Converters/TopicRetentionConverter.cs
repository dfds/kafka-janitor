using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class TopicRetentionConverter : ValueConverter<TopicRetention, uint>
{
    public TopicRetentionConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<TopicRetention, uint>> ToDatabaseType => value => value.ToMilliseconds();
    private static Expression<Func<uint, TopicRetention>> FromDatabaseType => value => TopicRetention.FromMilliseconds(value);
}