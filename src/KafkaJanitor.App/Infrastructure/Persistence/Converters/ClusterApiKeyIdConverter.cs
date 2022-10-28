using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class ClusterApiKeyIdConverter : ValueConverter<ClusterApiKeyId, Guid>
{
    public ClusterApiKeyIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<ClusterApiKeyId, Guid>> ToDatabaseType => value => value;
    private static Expression<Func<Guid, ClusterApiKeyId>> FromDatabaseType => value => value;
}