using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class ServiceAccountIdConverter : ValueConverter<ServiceAccountId, string>
{
    public ServiceAccountIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<ServiceAccountId, string>> ToDatabaseType => value => value.ToString();
    private static Expression<Func<string, ServiceAccountId>> FromDatabaseType => value => ServiceAccountId.Parse(value);
}