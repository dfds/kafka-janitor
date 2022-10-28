using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class TopicPartitionConverter : ValueConverter<TopicPartition, uint>
{
    public TopicPartitionConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<TopicPartition, uint>> ToDatabaseType => value => value.ToNumber();
    private static Expression<Func<uint, TopicPartition>> FromDatabaseType => value => TopicPartition.From(value);
}