using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class GrantCapabilityAccessProcessIdConverter : ValueConverter<GrantCapabilityAccessProcessId, Guid>
{
    public GrantCapabilityAccessProcessIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<GrantCapabilityAccessProcessId, Guid>> ToDatabaseType => id => Guid.Parse(id.ToString());
    private static Expression<Func<Guid, GrantCapabilityAccessProcessId>> FromDatabaseType => value => GrantCapabilityAccessProcessId.Parse(value.ToString());
}