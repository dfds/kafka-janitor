using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class CapabilityRootIdConverter : ValueConverter<CapabilityRootId, string>
{
    public CapabilityRootIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<CapabilityRootId, string>> ToDatabaseType => value => value.ToString();
    private static Expression<Func<string, CapabilityRootId>> FromDatabaseType => value => CapabilityRootId.Parse(value);
}