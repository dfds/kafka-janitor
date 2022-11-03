using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class ClusterAccessIdConverter : ValueConverter<ClusterAccessId, Guid>
{
    public ClusterAccessIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<ClusterAccessId, Guid>> ToDatabaseType => value => value;
    private static Expression<Func<Guid, ClusterAccessId>> FromDatabaseType => value => value;
}