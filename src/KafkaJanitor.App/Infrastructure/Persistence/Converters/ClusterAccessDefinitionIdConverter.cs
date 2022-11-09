using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class ClusterAccessDefinitionIdConverter : ValueConverter<ClusterAccessDefinitionId, Guid>
{
    public ClusterAccessDefinitionIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<ClusterAccessDefinitionId, Guid>> ToDatabaseType => value => value;
    private static Expression<Func<Guid, ClusterAccessDefinitionId>> FromDatabaseType => value => value;
}